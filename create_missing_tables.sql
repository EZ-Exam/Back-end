-- Script to create missing tables for subscription system
-- Run this script in your database to create the required tables

-- 1. Create user_usage_tracking table
CREATE TABLE IF NOT EXISTS user_usage_tracking (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "SubscriptionTypeId" INTEGER NOT NULL,
    "UsageType" VARCHAR(50) NOT NULL,
    "UsageCount" INTEGER NOT NULL DEFAULT 0,
    "ResetDate" TIMESTAMP NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_user_usage_tracking_user 
        FOREIGN KEY ("UserId") REFERENCES users("Id") ON DELETE CASCADE,
    CONSTRAINT fk_user_usage_tracking_subscription_type 
        FOREIGN KEY ("SubscriptionTypeId") REFERENCES subscription_types("Id") ON DELETE CASCADE
);

-- 2. Create user_usage_history table
CREATE TABLE IF NOT EXISTS user_usage_history (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "UsageType" VARCHAR(50) NOT NULL,
    "ResourceId" INTEGER,
    "ResourceType" VARCHAR(50),
    "Description" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_user_usage_history_user 
        FOREIGN KEY ("UserId") REFERENCES users("Id") ON DELETE CASCADE
);

-- 3. Add missing columns to subscription_types table if they don't exist
ALTER TABLE subscription_types 
ADD COLUMN IF NOT EXISTS "MaxSolutionViews" INTEGER DEFAULT 0,
ADD COLUMN IF NOT EXISTS "MaxAIRequests" INTEGER DEFAULT 0,
ADD COLUMN IF NOT EXISTS "IsAIEnabled" BOOLEAN DEFAULT false,
ADD COLUMN IF NOT EXISTS "Features" TEXT,
ADD COLUMN IF NOT EXISTS "IsActive" BOOLEAN DEFAULT true;

-- 4. Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_user_usage_tracking_user_subscription 
    ON user_usage_tracking("UserId", "SubscriptionTypeId", "UsageType");

CREATE INDEX IF NOT EXISTS idx_user_usage_tracking_reset_date 
    ON user_usage_tracking("ResetDate");

CREATE INDEX IF NOT EXISTS idx_user_usage_history_user_usage_type 
    ON user_usage_history("UserId", "UsageType");

CREATE INDEX IF NOT EXISTS idx_user_usage_history_created_at 
    ON user_usage_history("CreatedAt");

-- 5. Insert default subscription types if they don't exist
INSERT INTO subscription_types (
    "SubscriptionCode",
    "SubscriptionName", 
    "SubscriptionPrice",
    "Description",
    "MaxSolutionViews",
    "MaxAIRequests", 
    "IsAIEnabled",
    "Features",
    "IsActive",
    "CreatedAt",
    "UpdatedAt"
)
VALUES 
    ('FREE', 'Free Plan', 0.00, 'Basic access with limited features', 5, 0, false, 
     '{"solution_views": 5, "ai_access": false, "premium_features": false}', true, NOW(), NOW()),
    ('BASIC', 'Basic Plan', 99000.00, 'Enhanced access with AI features', 50, 10, true,
     '{"solution_views": 50, "ai_access": true, "ai_requests": 10, "premium_features": false}', true, NOW(), NOW()),
    ('PREMIUM', 'Premium Plan', 199000.00, 'Full access with unlimited solutions', -1, 100, true,
     '{"solution_views": -1, "ai_access": true, "ai_requests": 100, "premium_features": true}', true, NOW(), NOW()),
    ('UNLIMITED', 'Unlimited Plan', 299000.00, 'Complete access with no limits', -1, -1, true,
     '{"solution_views": -1, "ai_access": true, "ai_requests": -1, "premium_features": true, "priority_support": true}', true, NOW(), NOW())
ON CONFLICT ("SubscriptionCode") DO UPDATE SET
    "SubscriptionName" = EXCLUDED."SubscriptionName",
    "SubscriptionPrice" = EXCLUDED."SubscriptionPrice", 
    "Description" = EXCLUDED."Description",
    "MaxSolutionViews" = EXCLUDED."MaxSolutionViews",
    "MaxAIRequests" = EXCLUDED."MaxAIRequests",
    "IsAIEnabled" = EXCLUDED."IsAIEnabled",
    "Features" = EXCLUDED."Features",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = NOW();

-- 6. Assign FREE subscription to all existing users who don't have any subscription
INSERT INTO user_subscriptions (
    "UserId",
    "SubscriptionTypeId", 
    "Amount",
    "PaymentStatus",
    "PaymentGatewayTransactionId",
    "StartDate",
    "EndDate",
    "IsActive",
    "CreatedAt",
    "UpdatedAt"
)
SELECT 
    u."Id" as "UserId",
    st."Id" as "SubscriptionTypeId",
    0 as "Amount",
    'Completed' as "PaymentStatus", 
    CONCAT('AUTO_FREE_', u."Id", '_', EXTRACT(EPOCH FROM NOW())) as "PaymentGatewayTransactionId",
    NOW() as "StartDate",
    NULL as "EndDate",
    true as "IsActive",
    NOW() as "CreatedAt",
    NOW() as "UpdatedAt"
FROM users u
CROSS JOIN subscription_types st
WHERE st."SubscriptionCode" = 'FREE'
AND NOT EXISTS (
    SELECT 1 FROM user_subscriptions us 
    WHERE us."UserId" = u."Id" AND us."IsActive" = true
);

-- 7. Verify the setup
SELECT 'Setup completed successfully!' as status;

-- Show subscription types
SELECT 
    "SubscriptionCode",
    "SubscriptionName",
    "SubscriptionPrice",
    "MaxSolutionViews", 
    "MaxAIRequests",
    "IsAIEnabled",
    "IsActive"
FROM subscription_types 
ORDER BY "SubscriptionPrice";

-- Show user subscription count
SELECT 
    COUNT(*) as total_users,
    COUNT(CASE WHEN us."IsActive" = true THEN 1 END) as users_with_active_subscription
FROM users u
LEFT JOIN user_subscriptions us ON u."Id" = us."UserId" AND us."IsActive" = true;
