namespace KPITrackerAPI.Authorization;

public static class AppPermissions
{
    public const string ViewDashboard = "ViewDashboard";
    public const string ViewCatpIndicatorReport = "ViewCatpIndicatorReport";

    public const string ManageIndicatorCatalog = "ManageIndicatorCatalog";
    public const string ManageUnitCatalog = "ManageUnitCatalog";
    public const string ManageReportingPeriods = "ManageReportingPeriods";

    public const string ManageAssignmentWaves = "ManageAssignmentWaves";
    public const string AssignTargetsToCatp = "AssignTargetsToCatp";
    public const string AssignTargetsToPhong = "AssignTargetsToPhong";
    public const string AssignTargetsToCadp = "AssignTargetsToCadp";
    public const string ViewAssignedTargetsList = "ViewAssignedTargetsList";

    public const string SubmitPeriodicReports = "SubmitPeriodicReports";
    public const string ViewExecutionProgress = "ViewExecutionProgress";
    public const string ViewUnitsPendingUpdate = "ViewUnitsPendingUpdate";

    public const string ConfigureEvaluationThresholds = "ConfigureEvaluationThresholds";
    public const string ViewAccumulatedEvaluation = "ViewAccumulatedEvaluation";
    public const string ViewRiskWarnings = "ViewRiskWarnings";
    public const string CompareUnits = "CompareUnits";
    public const string RankUnits = "RankUnits";
    public const string ConfigureCompetitionGroups = "ConfigureCompetitionGroups";
    public const string ViewCompetitionGroups = "ViewCompetitionGroups";

    public const string ViewSummaryReports = "ViewSummaryReports";
    public const string ViewReportsByUnit = "ViewReportsByUnit";
    public const string ViewReportsByIndicator = "ViewReportsByIndicator";
    public const string ExportReports = "ExportReports";

    public const string ManageUsers = "ManageUsers";
    public const string ManagePermissions = "ManagePermissions";
    public const string ViewSystemLogs = "ViewSystemLogs";
    public const string ResetUserPasswords = "ResetUserPasswords";

    public static readonly string[] All =
    {
        ViewDashboard,
        ViewCatpIndicatorReport,
        ManageIndicatorCatalog,
        ManageUnitCatalog,
        ManageReportingPeriods,
        ManageAssignmentWaves,
        AssignTargetsToCatp,
        AssignTargetsToPhong,
        AssignTargetsToCadp,
        ViewAssignedTargetsList,
        SubmitPeriodicReports,
        ViewExecutionProgress,
        ViewUnitsPendingUpdate,
        ConfigureEvaluationThresholds,
        ViewAccumulatedEvaluation,
        ViewRiskWarnings,
        CompareUnits,
        RankUnits,
        ConfigureCompetitionGroups,
        ViewCompetitionGroups,
        ViewSummaryReports,
        ViewReportsByUnit,
        ViewReportsByIndicator,
        ExportReports,
        ManageUsers,
        ManagePermissions,
        ViewSystemLogs,
        ResetUserPasswords
    };
}
