using FluentMigrator;
using OrderService.Entities;

namespace OrderService.Postgres.Migrations;

[Migration(20250126, TransactionBehavior.Default, "Add order and transactional outbox table")]
public class AddOrderAndOutboxTable : Migration
{
    public override void Up()
    {
        Create.Table("Order")
            .WithColumn("Prefix").AsInt16().NotNullable().WithDefaultValue(ServiceConstants.ServiceAssignedPrefix)
            .WithColumn("Id").AsInt64().NotNullable().Unique().Identity()
            .WithColumn("CustomerId").AsInt64().NotNullable()
            .WithColumn("TotalAmount").AsDecimal().NotNullable()
            .WithColumn("OrderStatus").AsInt16().NotNullable().WithDefaultValue(OrderStatus.Created)
            .WithColumn("Items").AsCustom("JSON").NotNullable()
            .WithColumn("CreatedTime").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("UpdatedTime").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);

        Create.PrimaryKey("PK_Order").OnTable("Order").Columns("Prefix", "Id");

        Create.Table("Outbox")
            .WithColumn("Id").AsInt64().PrimaryKey().Unique().Identity()
            .WithColumn("EventType").AsString(ServiceConstants.StringMaxLength).NotNullable()
            .WithColumn("Payload").AsCustom("JSON").NotNullable()
            .WithColumn("CreatedTime").AsDateTimeOffset().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("ProcessedTime").AsDateTimeOffset().Nullable()
            .WithColumn("Status").AsInt16().NotNullable().WithDefaultValue(MessageStatus.Pending)
            .WithColumn("RetryCount").AsInt16().NotNullable().WithDefaultValue(0);
    }

    public override void Down()
    {
        Delete.Table("Order");
        Delete.Table("Outbox");
    }
}