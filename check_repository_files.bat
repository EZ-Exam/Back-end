@echo off
echo Checking repository files for missing interfaces...

echo.
echo Checking SubscriptionTypeRepository...
findstr /C:"ISubscriptionTypeRepository" "teamseven.EzExam.Repository\Repository\SubscriptionTypeRepository.cs"
if %errorlevel% neq 0 (
    echo ERROR: SubscriptionTypeRepository missing ISubscriptionTypeRepository interface
) else (
    echo OK: SubscriptionTypeRepository has interface
)

echo.
echo Checking UserUsageTrackingRepository...
findstr /C:"IUserUsageTrackingRepository" "teamseven.EzExam.Repository\Repository\UserUsageTrackingRepository.cs"
if %errorlevel% neq 0 (
    echo ERROR: UserUsageTrackingRepository missing IUserUsageTrackingRepository interface
) else (
    echo OK: UserUsageTrackingRepository has interface
)

echo.
echo Checking UserUsageHistoryRepository...
findstr /C:"IUserUsageHistoryRepository" "teamseven.EzExam.Repository\Repository\UserUsageHistoryRepository.cs"
if %errorlevel% neq 0 (
    echo ERROR: UserUsageHistoryRepository missing IUserUsageHistoryRepository interface
) else (
    echo OK: UserUsageHistoryRepository has interface
)

echo.
echo Checking UserSocialProviderRepository...
findstr /C:"IUserSocialProviderRepository" "teamseven.EzExam.Repository\Repository\UserSocialProviderRepository.cs"
if %errorlevel% neq 0 (
    echo ERROR: UserSocialProviderRepository missing IUserSocialProviderRepository interface
) else (
    echo OK: UserSocialProviderRepository has interface
)

echo.
echo Checking if IGenericRepository exists...
if exist "teamseven.EzExam.Repository\Basic\IGenericRepository.cs" (
    echo OK: IGenericRepository.cs exists
) else (
    echo ERROR: IGenericRepository.cs missing
)

echo.
echo Done checking!
pause
