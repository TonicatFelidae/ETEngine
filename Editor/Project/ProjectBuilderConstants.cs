#if UNITY_EDITOR
namespace ETEngine.Editor
{
    internal static class ProjectBuilderConstants
    {
        // ── Target directories ────────────────────────────────────────────
        public const string DirGeneralObject = "Assets/Game/___Script___";
        public const string DirUI = "Assets/Game/___Script___/UI";
        public const string DirInstaller = "Assets/Game/___Script___/Installer";
        public const string DirAnimation = "Assets/Game/___Script___/Animation";
        public const string DirData = "Assets/Game/___Script___/Data";

        // ── Class name suffixes ───────────────────────────────────────────
        public const string SuffixGeneralObject = "GeneralObject";
        public const string SuffixPage = "Screen";
        public const string SuffixPopup = "Popup";
        public const string SuffixSheet = "Sheet";
        public const string SuffixInstaller = "Installer";
        public const string SuffixObjectAnimatorController = "ObjectAnimatorController";
    }
}
#endif
