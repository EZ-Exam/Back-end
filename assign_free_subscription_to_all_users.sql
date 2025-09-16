-- Script to assign FREE subscription to all existing users
-- This ensures all users have a subscription and can be tracked for AI limits

-- First, let's check if FREE subscription type exists, if not create it
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
SELECT 
    'FREE',
    'Free Plan',
    0.00,
    'Free subscription with limited features - 5 solution views per day, no AI access',
    5,  -- 5 solution views per day
    0,  -- 0 AI requests (no AI access)
    false, -- AI not enabled
    '{"solution_views": 5, "ai_access": false, "premium_features": false}',
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
WHERE NOT EXISTS (
    SELECT 1 FROM subscription_types WHERE "SubscriptionCode" = 'FREE'
);

-- Get the FREE subscription type ID
DO $$
DECLARE
    free_subscription_id INTEGER;
    user_record RECORD;
    existing_subscription_count INTEGER;
BEGIN
    -- Get the FREE subscription type ID
    SELECT "Id" INTO free_subscription_id 
    FROM subscription_types 
    WHERE "SubscriptionCode" = 'FREE';
    
    IF free_subscription_id IS NULL THEN
        RAISE EXCEPTION 'FREE subscription type not found or could not be created';
    END IF;
    
    RAISE NOTICE 'FREE subscription type ID: %', free_subscription_id;
    
    -- Assign FREE subscription to all users who don't have any active subscription
    FOR user_record IN 
        SELECT u."Id" as user_id, u."Email" as user_email
        FROM users u
        WHERE NOT EXISTS (
            SELECT 1 
            FROM user_subscriptions us 
            WHERE us."UserId" = u."Id" 
            AND us."IsActive" = true 
            AND us."PaymentStatus" = 'Completed'
        )
    LOOP
        -- Check if user already has any subscription (even inactive)
        SELECT COUNT(*) INTO existing_subscription_count
        FROM user_subscriptions 
        WHERE "UserId" = user_record.user_id;
        
        -- Insert FREE subscription for user
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
        ) VALUES (
            user_record.user_id,
            free_subscription_id,
            0.00,
            'Completed', -- Free subscription is automatically completed
            'FREE_AUTO_ASSIGNED_' || user_record.user_id,
            CURRENT_TIMESTAMP,
            NULL, -- No end date for free subscription
            true,
            CURRENT_TIMESTAMP,
            CURRENT_TIMESTAMP
        );
        
        RAISE NOTICE 'Assigned FREE subscription to user: % (%)', user_record.user_email, user_record.user_id;
    END LOOP;
    
    -- Show summary
    RAISE NOTICE '=== SUBSCRIPTION ASSIGNMENT SUMMARY ===';
    RAISE NOTICE 'Total users with FREE subscription: %', (
        SELECT COUNT(*) 
        FROM user_subscriptions us 
        JOIN subscription_types st ON us."SubscriptionTypeId" = st."Id"
        WHERE st."SubscriptionCode" = 'FREE' AND us."IsActive" = true
    );
    
    RAISE NOTICE 'Total users with any active subscription: %', (
        SELECT COUNT(DISTINCT "UserId") 
        FROM user_subscriptions 
        WHERE "IsActive" = true AND "PaymentStatus" = 'Completed'
    );
    
    RAISE NOTICE 'Total users in system: %', (SELECT COUNT(*) FROM users);
END $$;

-- Verify the results
SELECT 
    st."SubscriptionCode",
    st."SubscriptionName",
    st."MaxSolutionViews",
    st."MaxAIRequests",
    st."IsAIEnabled",
    COUNT(us."Id") as user_count
FROM subscription_types st
LEFT JOIN user_subscriptions us ON st."Id" = us."SubscriptionTypeId" AND us."IsActive" = true
GROUP BY st."Id", st."SubscriptionCode", st."SubscriptionName", st."MaxSolutionViews", st."MaxAIRequests", st."IsAIEnabled"
ORDER BY st."Id";

-- Show users without any subscription (should be 0 after this script)
SELECT 
    u."Id",
    u."Email",
    u."FullName",
    'NO SUBSCRIPTION' as subscription_status
FROM users u
WHERE NOT EXISTS (
    SELECT 1 
    FROM user_subscriptions us 
    WHERE us."UserId" = u."Id" 
    AND us."IsActive" = true 
    AND us."PaymentStatus" = 'Completed'
)
ORDER BY u."Id";
