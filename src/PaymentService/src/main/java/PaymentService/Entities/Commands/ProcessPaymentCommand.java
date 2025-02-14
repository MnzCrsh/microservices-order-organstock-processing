package PaymentService.Entities.Commands;

import java.math.BigDecimal;
import java.time.ZonedDateTime;
import java.util.UUID;

public record ProcessPaymentCommand(
        UUID OrderId,
        PaymentStatus status,
        BigDecimal paymentAmount,
        ZonedDateTime processedAt) {
}
