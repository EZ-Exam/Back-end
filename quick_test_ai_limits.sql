-- Quick test script to verify AI limits system
-- Run this after assign_free_subscription_to_all_users.sql

-- 1. Check subscription types
SELECT 
    'SUBSCRIPTION TYPES' as test_section,
    "Id",
    "SubscriptionCode",
    "SubscriptionName",
    "MaxSolutionViews",
    "MaxAIRequests",
    "IsAIEnabled",
    "IsActive"
FROM subscription_types
ORDER BY "Id";

-- 2. Check user subscriptions
SELECT 
    'USER SUBSCRIPTIONS' as test_section,
    u."Id" as user_id,
    u."Email",
    st."SubscriptionCode",
    st."SubscriptionName",
    us."IsActive",
    us."PaymentStatus",
    st."IsAIEnabled",
    st."MaxAIRequests"
FROM user_subscriptions us
JOIN users u ON us."UserId" = u."Id"
JOIN subscription_types st ON us."SubscriptionTypeId" = st."Id"
WHERE us."IsActive" = true
ORDER BY u."Id";

-- 3. Check users without subscription (should be 0)
SELECT 
    'USERS WITHOUT SUBSCRIPTION' as test_section,
    u."Id",
    u."Email",
    u."FullName"
FROM users u
WHERE NOT EXISTS (
    SELECT 1 
    FROM user_subscriptions us 
    WHERE us."UserId" = u."Id" 
    AND us."IsActive" = true 
    AND us."PaymentStatus" = 'Completed'
)
ORDER BY u."Id";

-- 4. Summary statistics
SELECT 
    'SUMMARY STATISTICS' as test_section,
    (SELECT COUNT(*) FROM users) as total_users,
    (SELECT COUNT(*) FROM user_subscriptions WHERE "IsActive" = true) as active_subscriptions,
    (SELECT COUNT(*) FROM subscription_types WHERE "IsActive" = true) as active_subscription_types,
    (SELECT COUNT(*) FROM subscription_types WHERE "IsAIEnabled" = true) as ai_enabled_subscriptions,
    (SELECT COUNT(*) FROM subscription_types WHERE "SubscriptionCode" = 'FREE') as free_subscription_count;

-- 5. Test data for API testing
SELECT 
    'TEST DATA FOR API' as test_section,
    u."Id" as user_id,
    u."Email",
    st."SubscriptionCode",
    CASE 
        WHEN st."IsAIEnabled" = true THEN 'CAN_USE_AI'
        ELSE 'CANNOT_USE_AI'
    END as ai_access_status,
    st."MaxAIRequests" as ai_requests_limit,
    st."MaxSolutionViews" as solution_views_limit
FROM user_subscriptions us
JOIN users u ON us."UserId" = u."Id"
JOIN subscription_types st ON us."SubscriptionTypeId" = st."Id"
WHERE us."IsActive" = true
ORDER BY st."IsAIEnabled" DESC, u."Id"
LIMIT 10;
