using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orders.V1;
using OrderService.CQRS.Abstractions;
using ValidationError = OrderService.Validation.ValidationError;

namespace OrderService.gRPC;

public class OrderGrpcService(IOrderCommandProcessor processor, ILogger<OrderGrpcService> logger)
    : OrderCommandProcessor.OrderCommandProcessorBase
{
    public override async Task<OrderResponse> ExecuteCreateAsync(CreateOrderCommand request, ServerCallContext context)
    {
        try
        {
            var mappedRequest = MapProtobufToCreateOrderCommand(request);
            var (responseItem, errors) = await processor.ExecuteCreateAsync(mappedRequest);

            var result = OrderResponseToProtobufResponse(responseItem, errors);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error processing gRPC request. {Error}", e);
            throw new RpcException(new Status(StatusCode.Internal, $"Internal server error {e.Message}"));
        }
    }

    private static OrderResponse OrderResponseToProtobufResponse(
        Entities.Models.Responses.OrderResponseItem? responseItem, List<ValidationError>? errors)
    {
        var protobufResponseItem = new OrderResponseItem();
        if (responseItem is not null)
        {
            var (lo, hi, signScale) = DecimalConverter.ToProtobuf(responseItem.TotalAmount);
            protobufResponseItem = new OrderResponseItem
            {
                Id = responseItem.OrderId.ToString(),
                CustomerId = responseItem.CustomerId.ToString(),
                Status = (int)responseItem.Status,
                TotalAmount =
                {
                    Lo = lo,
                    Hi = hi,
                    SignScale = signScale
                }
            };
        }

        var protobufErrors = errors?.Select(e => new Orders.V1.ValidationError
        {
            PropertyName = e.PropertyName,
            ErrorMessage = e.ErrorMessage
        }).ToList();

        return new OrderResponse
        {
            Item = protobufResponseItem,
            Errors = { protobufErrors }
        };
    }

    private static Entities.Models.Commands.CreateOrderCommand MapProtobufToCreateOrderCommand(CreateOrderCommand request)
    {
        var mappedRequest = new Entities.Models.Commands.CreateOrderCommand
        {
            CustomerId = Guid.Parse(request.CustomerId),
            TotalAmount = DecimalConverter.FromProtobuf(
                request.TotalAmount.Lo,
                request.TotalAmount.Hi,
                request.TotalAmount.SignScale),
            Items = request.Items.Select(innerItem =>
            {
                var success = Guid.TryParse(innerItem, out var result);
                return new { success, result };
            })
                .Where(x => x.success)
                .Select(x => x.result)
                .ToArray()
        };
        return mappedRequest;
    }
}