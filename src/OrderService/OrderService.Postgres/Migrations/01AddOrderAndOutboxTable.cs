using FluentMigrator;
using OrderService.Entities;

namespace OrderService.Postgres.Migrations;

[Migration(20250126, TransactionBehavior.Default, "Add order and transactional outbox table")]
public class AddOrderAndOutboxTable : Migration
{
    public override void Up()
    {
        Create.Table("Order")
            
            .WithColumn("CombinedKey")
            .AsCustom("BIGINT GENERATED ALWAYS AS ((\"IdPrefix\" << 16) | (\"IdInfix\" << 8) | \"IdPostfix\") STORED")
            .NotNullable()
            .Unique()
            
            .WithColumn("IdPrefix").AsByte().NotNullable().Identity()
            .WithColumn("IdInfix").AsInt64().NotNullable()
            .WithColumn("IdPostfix").AsByte().NotNullable().WithDefaultValue(ServiceConstants.ServiceAssignedNumber)
            
            .WithColumn("CustomerId").AsGuid().NotNullable()
            .WithColumn("TotalAmount").AsDecimal().NotNullable()
            .WithColumn("OrderStatus").AsInt16().NotNullable().WithDefaultValue((int)OrderStatus.Created)
            .WithColumn("Items").AsCustom("JSON").NotNullable()
            .WithColumn("CreatedTime").AsDateTime().NotNullable()
            .WithColumn("UpdatedTime").AsDateTime().NotNullable();
        
        Create.PrimaryKey("PK_Order")
            .OnTable("Order")
            .Columns("CombinedKey");

        Create.Table("Outbox")
            .WithColumn("Id").AsInt64().PrimaryKey().Unique().Identity()
            .WithColumn("EventType").AsString(ServiceConstants.StringMaxLength).NotNullable()
            .WithColumn("Payload").AsCustom("JSON").NotNullable()
            .WithColumn("CreatedTime").AsDateTimeOffset().NotNullable()
            .WithColumn("ProcessedTime").AsDateTimeOffset().Nullable()
            .WithColumn("Status").AsInt16().NotNullable().WithDefaultValue((int)MessageStatus.Pending)
            .WithColumn("RetryCount").AsInt16().NotNullable().WithDefaultValue(0);
    }

    public override void Down()
    {
        Delete.Table("Order");
        Delete.Table("Outbox");
    }
}