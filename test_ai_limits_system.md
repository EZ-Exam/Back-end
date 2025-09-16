# Test AI Limits System

## 🎯 **Mục tiêu test:**
Kiểm tra hệ thống limit AI cho free user hoạt động đúng

## 📋 **Các bước test:**

### **1. Chuẩn bị dữ liệu:**
```sql
-- Chạy script này trước để gán free subscription cho tất cả user
\i assign_free_subscription_to_all_users.sql
```

### **2. Test với Free User (AI bị block):**

#### **Test 1: Gọi AI endpoint với free user**
```bash
# Lấy JWT token của free user
curl -X POST "http://localhost:5000/api/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "freeuser@example.com",
    "password": "password123"
  }'

# Test gọi Gemini 1.5 (sẽ bị block)
curl -X POST "http://localhost:5000/api/gemini-15/solve" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "question": "What is the speed of light?",
    "subject": "Physics"
  }'
```

**Kết quả mong đợi:**
```json
{
  "message": "AI access is not available with your current subscription. Please upgrade to a premium plan.",
  "errorCode": "SUBSCRIPTION_LIMIT_EXCEEDED",
  "subscriptionRequired": true
}
```

#### **Test 2: Kiểm tra usage tracking**
```bash
# Kiểm tra usage status
curl -X GET "http://localhost:5000/api/usage-tracking/status" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Kết quả mong đợi:**
```json
{
  "userId": 1,
  "subscriptionType": "FREE",
  "subscriptionName": "Free Plan",
  "isAIEnabled": false,
  "maxAIRequests": 0,
  "currentAIRequests": 0,
  "maxSolutionViews": 5,
  "currentSolutionViews": 0,
  "canPerformAction": false,
  "message": "AI access not available with current subscription"
}
```

### **3. Test với Premium User (AI được phép):**

#### **Tạo Premium User:**
```sql
-- Tạo premium subscription type
INSERT INTO subscription_types (
    "SubscriptionCode", 
    "SubscriptionName", 
    "SubscriptionPrice", 
    "Description", 
    "MaxSolutionViews", 
    "MaxAIRequests", 
    "IsAIEnabled", 
    "Features", 
    "IsActive"
) VALUES (
    'PREMIUM',
    'Premium Plan',
    299000.00,
    'Premium subscription with unlimited AI access and solution views',
    -1,  -- unlimited solution views
    -1,  -- unlimited AI requests
    true, -- AI enabled
    '{"solution_views": -1, "ai_access": true, "premium_features": true}',
    true
);

-- Gán premium subscription cho user
INSERT INTO user_subscriptions (
    "UserId",
    "SubscriptionTypeId",
    "Amount",
    "PaymentStatus",
    "PaymentGatewayTransactionId",
    "StartDate",
    "EndDate",
    "IsActive"
) VALUES (
    1, -- User ID
    (SELECT "Id" FROM subscription_types WHERE "SubscriptionCode" = 'PREMIUM'),
    299000.00,
    'Completed',
    'PREMIUM_TEST_123',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP + INTERVAL '30 days',
    true
);
```

#### **Test Premium User:**
```bash
# Test gọi AI với premium user
curl -X POST "http://localhost:5000/api/gemini-15/solve" \
  -H "Authorization: Bearer PREMIUM_USER_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "question": "What is the speed of light?",
    "subject": "Physics"
  }'
```

**Kết quả mong đợi:** AI response thành công

### **4. Test Solution Views Limit:**

#### **Test với Free User:**
```bash
# Gọi solution endpoint 6 lần (vượt quá limit 5)
for i in {1..6}; do
  curl -X GET "http://localhost:5000/api/solutions/1" \
    -H "Authorization: Bearer FREE_USER_JWT_TOKEN"
done
```

**Kết quả mong đợi:** Lần thứ 6 sẽ bị block

### **5. Test Middleware Logs:**

Kiểm tra logs để đảm bảo middleware hoạt động:
```bash
# Xem logs của application
tail -f logs/app.log | grep "SubscriptionMiddleware"
```

**Logs mong đợi:**
```
[INFO] User 1 successfully accessed AI endpoint: /api/gemini-15/solve
[WARN] User 2 attempted to access AI endpoint /api/gemini-15/solve but subscription does not allow
```

## 🔍 **Kiểm tra Database:**

### **1. Kiểm tra subscription data:**
```sql
-- Xem tất cả subscription types
SELECT 
    "Id",
    "SubscriptionCode",
    "SubscriptionName",
    "MaxSolutionViews",
    "MaxAIRequests",
    "IsAIEnabled",
    "IsActive"
FROM subscription_types
ORDER BY "Id";

-- Xem user subscriptions
SELECT 
    u."Email",
    st."SubscriptionCode",
    st."SubscriptionName",
    us."IsActive",
    us."PaymentStatus"
FROM user_subscriptions us
JOIN users u ON us."UserId" = u."Id"
JOIN subscription_types st ON us."SubscriptionTypeId" = st."Id"
WHERE us."IsActive" = true
ORDER BY u."Id";
```

### **2. Kiểm tra usage tracking:**
```sql
-- Xem usage tracking data
SELECT 
    u."Email",
    ut."UsageType",
    ut."UsageCount",
    ut."ResetDate"
FROM user_usage_tracking ut
JOIN users u ON ut."UserId" = u."Id"
ORDER BY ut."CreatedAt" DESC;

-- Xem usage history
SELECT 
    u."Email",
    uh."UsageType",
    uh."Description",
    uh."CreatedAt"
FROM user_usage_history uh
JOIN users u ON uh."UserId" = u."Id"
ORDER BY uh."CreatedAt" DESC
LIMIT 20;
```

## ✅ **Checklist Test:**

- [ ] Free user bị block khi gọi AI endpoints
- [ ] Premium user có thể gọi AI endpoints
- [ ] Usage tracking được ghi lại đúng
- [ ] Solution views limit hoạt động
- [ ] Middleware logs đúng
- [ ] Database data consistent
- [ ] Error messages rõ ràng
- [ ] JWT authentication hoạt động

## 🚨 **Troubleshooting:**

### **Nếu AI vẫn hoạt động với free user:**
1. Kiểm tra middleware có được đăng ký đúng không
2. Kiểm tra subscription data trong database
3. Kiểm tra JWT token có đúng user ID không

### **Nếu premium user bị block:**
1. Kiểm tra subscription type có `IsAIEnabled = true` không
2. Kiểm tra user subscription có `IsActive = true` không
3. Kiểm tra payment status có `Completed` không

### **Nếu middleware không chạy:**
1. Kiểm tra Program.cs có `app.UseSubscriptionMiddleware()` không
2. Kiểm tra service registration
3. Kiểm tra logs để xem có error không
