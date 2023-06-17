using JetBrains.Annotations;

namespace Map
{
    public readonly struct GridWrapper
    {
        public readonly bool HasGridInt;
        [CanBeNull] public readonly GridInt GridInt;
        public readonly bool HasGridSector;
        [CanBeNull] public readonly GridPlayerSector GridPlayerSector;

        public int GetWidth()
        {
            if (HasGridInt)
            {
                return GridInt!.GetWidth();
            }

            if (HasGridSector)
            {
                return GridPlayerSector!.GetWidth();
            }

            return -1;
        }
        
        public int GetLength()
        {
            if (HasGridInt)
            {
                return GridInt!.GetLength();
            }

            if (HasGridSector)
            {
                return GridPlayerSector!.GetLength();
            }

            return -1;
        }
        
        public static GridWrapper CreateGridInt(GridInt gridInt)
        {
            return new GridWrapper(gridInt, null);
        }

        public static GridWrapper CreateGridSector(GridPlayerSector gridPlayerSector)
        {
            return new GridWrapper(null, gridPlayerSector);
        }

        private GridWrapper(GridInt gridInt, GridPlayerSector gridPlayerSector)
        {
            GridInt = gridInt;
            GridPlayerSector = gridPlayerSector;

            HasGridInt = GridInt != null;
            HasGridSector = GridPlayerSector != null;
        }
    }
}