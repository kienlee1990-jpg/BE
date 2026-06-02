namespace KPITrackerAPI.Authorization;

public static class AppPermissions
{
    public const string ViewDashboard = "ViewDashboard";
    public const string ViewCatpIndicatorReport = "ViewCatpIndicatorReport";

    public const string ManageIndicatorCatalog = "ManageIndicatorCatalog";
    public const string CreateIndicatorCatalog = "CreateIndicatorCatalog";
    public const string ImportIndicatorCatalog = "ImportIndicatorCatalog";
    public const string AssignIndicatorCatalogOwner = "AssignIndicatorCatalogOwner";
    public const string EditIndicatorCatalog = "EditIndicatorCatalog";
    public const string DeleteIndicatorCatalog = "DeleteIndicatorCatalog";
    public const string ManageUnitCatalog = "ManageUnitCatalog";
    public const string CreateUnitCatalog = "CreateUnitCatalog";
    public const string ImportUnitCatalog = "ImportUnitCatalog";
    public const string EditUnitCatalog = "EditUnitCatalog";
    public const string DeleteUnitCatalog = "DeleteUnitCatalog";
    public const string ManageReportingPeriods = "ManageReportingPeriods";
    public const string CreateReportingPeriods = "CreateReportingPeriods";
    public const string EditReportingPeriods = "EditReportingPeriods";
    public const string DeleteReportingPeriods = "DeleteReportingPeriods";

    public const string ManageAssignmentWaves = "ManageAssignmentWaves";
    public const string CreateAssignmentWaves = "CreateAssignmentWaves";
    public const string EditAssignmentWaves = "EditAssignmentWaves";
    public const string DeleteAssignmentWaves = "DeleteAssignmentWaves";
    public const string AssignTargetsToCatp = "AssignTargetsToCatp";
    public const string AssignTargetsToPhong = "AssignTargetsToPhong";
    public const string AssignTargetsToCadp = "AssignTargetsToCadp";
    public const string CreateAssignedTargets = "CreateAssignedTargets";
    public const string EditAssignedTargets = "EditAssignedTargets";
    public const string DeleteAssignedTargets = "DeleteAssignedTargets";
    public const string ViewAssignedTargetsList = "ViewAssignedTargetsList";

    public const string SubmitPeriodicReports = "SubmitPeriodicReports";
    public const string CreatePeriodicReports = "CreatePeriodicReports";
    public const string SendPeriodicReports = "SendPeriodicReports";
    public const string EditPeriodicReports = "EditPeriodicReports";
    public const string DeletePeriodicReports = "DeletePeriodicReports";
    public const string ViewReturnedReports = "ViewReturnedReports";
    public const string ResubmitReturnedReports = "ResubmitReturnedReports";
    public const string DeleteReturnedReports = "DeleteReturnedReports";
    public const string ReviewPendingReports = "ReviewPendingReports";
    public const string ViewPendingReportDetails = "ViewPendingReportDetails";
    public const string ApprovePendingReports = "ApprovePendingReports";
    public const string ReturnPendingReports = "ReturnPendingReports";
    public const string ViewExecutionProgress = "ViewExecutionProgress";
    public const string ViewUnitsPendingUpdate = "ViewUnitsPendingUpdate";

    public const string ConfigureEvaluationThresholds = "ConfigureEvaluationThresholds";
    public const string CreateEvaluationThresholds = "CreateEvaluationThresholds";
    public const string EditEvaluationThresholds = "EditEvaluationThresholds";
    public const string DeleteEvaluationThresholds = "DeleteEvaluationThresholds";
    public const string ViewAccumulatedEvaluation = "ViewAccumulatedEvaluation";
    public const string ViewRiskWarnings = "ViewRiskWarnings";
    public const string CompareUnits = "CompareUnits";
    public const string RankUnits = "RankUnits";
    public const string ConfigureCompetitionGroups = "ConfigureCompetitionGroups";
    public const string CreateCompetitionGroups = "CreateCompetitionGroups";
    public const string EditCompetitionGroups = "EditCompetitionGroups";
    public const string DeleteCompetitionGroups = "DeleteCompetitionGroups";
    public const string ViewCompetitionGroups = "ViewCompetitionGroups";

    public const string ViewSummaryReports = "ViewSummaryReports";
    public const string ViewReportsByUnit = "ViewReportsByUnit";
    public const string ViewReportsByIndicator = "ViewReportsByIndicator";
    public const string ExportReports = "ExportReports";

    public const string ManageUsers = "ManageUsers";
    public const string CreateUsers = "CreateUsers";
    public const string EditUsers = "EditUsers";
    public const string ManageUserRoles = "ManageUserRoles";
    public const string CreateRoles = "CreateRoles";
    public const string EditRoles = "EditRoles";
    public const string DeleteRoles = "DeleteRoles";
    public const string ManagePermissions = "ManagePermissions";
    public const string ViewSystemLogs = "ViewSystemLogs";
    public const string ResetUserPasswords = "ResetUserPasswords";

    public static readonly string[] All =
    {
        ViewDashboard,
        ViewCatpIndicatorReport,
        ManageIndicatorCatalog,
        CreateIndicatorCatalog,
        ImportIndicatorCatalog,
        AssignIndicatorCatalogOwner,
        EditIndicatorCatalog,
        DeleteIndicatorCatalog,
        ManageUnitCatalog,
        CreateUnitCatalog,
        ImportUnitCatalog,
        EditUnitCatalog,
        DeleteUnitCatalog,
        ManageReportingPeriods,
        CreateReportingPeriods,
        EditReportingPeriods,
        DeleteReportingPeriods,
        ManageAssignmentWaves,
        CreateAssignmentWaves,
        EditAssignmentWaves,
        DeleteAssignmentWaves,
        AssignTargetsToCatp,
        AssignTargetsToPhong,
        AssignTargetsToCadp,
        CreateAssignedTargets,
        EditAssignedTargets,
        DeleteAssignedTargets,
        ViewAssignedTargetsList,
        SubmitPeriodicReports,
        CreatePeriodicReports,
        SendPeriodicReports,
        EditPeriodicReports,
        DeletePeriodicReports,
        ViewReturnedReports,
        ResubmitReturnedReports,
        DeleteReturnedReports,
        ReviewPendingReports,
        ViewPendingReportDetails,
        ApprovePendingReports,
        ReturnPendingReports,
        ViewExecutionProgress,
        ViewUnitsPendingUpdate,
        ConfigureEvaluationThresholds,
        CreateEvaluationThresholds,
        EditEvaluationThresholds,
        DeleteEvaluationThresholds,
        ViewAccumulatedEvaluation,
        ViewRiskWarnings,
        CompareUnits,
        RankUnits,
        ConfigureCompetitionGroups,
        CreateCompetitionGroups,
        EditCompetitionGroups,
        DeleteCompetitionGroups,
        ViewCompetitionGroups,
        ViewSummaryReports,
        ViewReportsByUnit,
        ViewReportsByIndicator,
        ExportReports,
        ManageUsers,
        CreateUsers,
        EditUsers,
        ManageUserRoles,
        CreateRoles,
        EditRoles,
        DeleteRoles,
        ManagePermissions,
        ViewSystemLogs,
        ResetUserPasswords
    };
}
