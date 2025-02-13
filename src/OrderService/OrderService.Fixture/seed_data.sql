INSERT INTO public."Outbox" ("Id", "EventType", "Payload", "CreatedTime", "ProcessedTime", "Status", "RetryCount")
VALUES  ('a1e8e9b8-1c2f-4d66-9d3f-9e4f2b7e1a11', 'UserCreated', '{"userId": 1, "action": "create"}', '2025-02-12T16:00:00+00', NULL, 1, 0),
        ('b2f9ea88-2d3f-5e77-ad4f-8f5f3c8d2b22', 'OrderPlaced', '{"orderId": 101, "amount": 50.0}', '2025-02-12T16:01:00+00', NULL, 1, 0),
        ('c3fae998-3e4f-6f88-be5f-9f6f4d9e3c33', 'PaymentReceived', '{"paymentId": 201, "status": "completed"}', '2025-02-12T16:02:00+00', NULL, 1, 0),
        ('d4fbaa99-4e5f-7f99-cf60-0f7f5eaf4d44', 'UserUpdated', '{"userId": 2, "changes": {"email": "new@mail.com"}}', '2025-02-12T16:03:00+00', NULL, 1, 0),
        ('e5fcabb0-5f60-8faa-df71-1f807fb05e55', 'OrderCancelled', '{"orderId": 102, "reason": "out of stock"}', '2025-02-12T16:04:00+00', NULL, 1, 0),
        ('f6fdbcc1-6f71-9fbb-ef82-2f918fc16f66', 'InventoryUpdated', '{"itemId": 55, "quantity": 150}', '2025-02-12T16:05:00+00', NULL, 1, 0),
        ('07a1bcd2-7f82-afcc-f093-3fa29fd27f77', 'ProductAdded', '{"productId": 300, "name": "Widget"}', '2025-02-12T16:06:00+00', NULL, 1, 0),
        ('18b2cde3-8f93-b0dd-0a14-4fb3a0e38a88', 'DiscountApplied', '{"orderId": 103, "discount": 10}', '2025-02-12T16:07:00+00', NULL, 1, 0),
        ('29c3def4-9fa4-c1ee-1b25-5fc4b1f49b99', 'ShippingScheduled', '{"orderId": 104, "date": "2025-02-14"}', '2025-02-12T16:08:00+00', NULL, 1, 0),
        ('3ad4ef05-afb5-d2ff-2c36-6cd5c203ac00', 'FeedbackReceived', '{"userId": 3, "rating": 5}', '2025-02-12T16:09:00+00', NULL, 1, 0);
