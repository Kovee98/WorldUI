namespace WorldUI {
    public static class WorldUIExtensions {
        public static bool HasFlag (this WorldUIRendererFilter self, WorldUIRendererFilter flag) {
            return (self & flag) == flag;
        }
    }
}
