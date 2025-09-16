@echo off
echo Adding new and modified files to git...

REM New files
git add teamseven.EzExam.Repository/Basic/IGenericRepository.cs
git add teamseven.EzExam.Services/Object/Requests/SubscribeRequest.cs
git add teamseven.EzExam.Services/Object/Responses/SubscribeResponse.cs
git add teamseven.EzExam.Services/Services/SubscriptionService/ISubscriptionService.cs
git add teamseven.EzExam.Services/Services/SubscriptionService/SubscriptionService.cs
git add teamseven.EzExam.API/Controllers/SubscriptionController.cs
git add teamseven.EzExam.API/Services/SubscriptionExpirationService.cs
git add create_missing_tables.sql
git add test_subscription_api.http
git add SUBSCRIPTION_SETUP_README.md

REM Modified files
git add teamseven.EzExam.Repository/Basic/GenericRepository.cs
git add teamseven.EzExam.Repository/Repository/ISubscriptionTypeRepository.cs
git add teamseven.EzExam.Repository/Repository/SubscriptionTypeRepository.cs
git add teamseven.EzExam.Repository/Repository/IUserUsageTrackingRepository.cs
git add teamseven.EzExam.Repository/Repository/UserUsageTrackingRepository.cs
git add teamseven.EzExam.Repository/Repository/IUserUsageHistoryRepository.cs
git add teamseven.EzExam.Repository/Repository/UserUsageHistoryRepository.cs
git add teamseven.EzExam.Repository/Repository/IUserSocialProviderRepository.cs
git add teamseven.EzExam.Repository/Repository/UserSocialProviderRepository.cs
git add teamseven.EzExam.Services/Services/ServiceProvider/IServiceProviders.cs
git add teamseven.EzExam.Services/Services/ServiceProvider/ServiceProviders.cs
git add teamseven.EzExam.API/Program.cs

echo Committing files...
git commit -m "Add subscription system with auto-expiration

- Add IGenericRepository interface and update GenericRepository
- Add SubscriptionType, UserUsageTracking, UserUsageHistory repositories
- Add SubscriptionService with auto-expiration to FREE plan
- Add SubscriptionController with JWT authentication
- Add background service for subscription expiration
- Add database migration script
- Add API test file and setup documentation"

echo Done! Files have been committed to git.
echo Your colleague can now pull the latest changes.
pause
