using FluentMigrator;
using OrderService.Entities;

namespace OrderService.Postgres.Migrations;

[Migration(20250126, TransactionBehavior.Default, "Add order and transactional outbox table")]
public class AddOrderAndOutboxTable : Migration
{
    public override void Up()
    {
        Create.Table("Order")
            .WithColumn("Id").AsGuid().PrimaryKey().Unique()
            .WithColumn("CustomerId").AsGuid().NotNullable()
            .WithColumn("TotalAmount").AsDecimal().NotNullable()
            .WithColumn("OrderStatus").AsInt16().NotNullable().WithDefaultValue((int)OrderStatus.Created)
            .WithColumn("Items").AsString().NotNullable()
            .WithColumn("CreatedTime").AsDateTimeOffset().NotNullable()
            .WithColumn("UpdatedTime").AsDateTimeOffset().NotNullable();

        Create.Index("IX_Order_CreatedTime")
            .OnTable("Order")
            .OnColumn("CreatedTime").Ascending();

        Create.Table("Outbox")
            .WithColumn("Id").AsGuid().PrimaryKey().Unique()
            .WithColumn("EventType").AsString(ServiceConstants.StringMaxLength).NotNullable()
            .WithColumn("Payload").AsString().NotNullable()
            .WithColumn("CreatedTime").AsDateTimeOffset().NotNullable()
            .WithColumn("ProcessedTime").AsDateTimeOffset().Nullable()
            .WithColumn("Status").AsInt16().NotNullable().WithDefaultValue((int)MessageStatus.Pending)
            .WithColumn("RetryCount").AsInt16().NotNullable().WithDefaultValue(0);

        Create.Index("IX_Outbox_Status_CreatedTime")
            .OnTable("Outbox")
            .OnColumn("Status").Ascending()
            .OnColumn("CreatedTime").Ascending();
    }

    public override void Down()
    {
        Delete.Table("Order");
        Delete.Table("Outbox");
    }
}