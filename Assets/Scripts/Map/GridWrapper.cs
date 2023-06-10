using JetBrains.Annotations;

namespace Map
{
    public readonly struct GridWrapper
    {
        public readonly bool HasGridInt;
        [CanBeNull] public readonly GridInt GridInt;
        public readonly bool HasGridSector;
        [CanBeNull] public readonly GridSector GridSector;

        public int GetWidth()
        {
            if (HasGridInt)
            {
                return GridInt!.GetWidth();
            }

            if (HasGridSector)
            {
                return GridSector!.GetWidth();
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
                return GridSector!.GetLength();
            }

            return -1;
        }
        
        public static GridWrapper CreateGridInt(GridInt gridInt)
        {
            return new GridWrapper(gridInt, null);
        }

        public static GridWrapper CreateGridSector(GridSector gridSector)
        {
            return new GridWrapper(null, gridSector);
        }

        private GridWrapper(GridInt gridInt, GridSector gridSector)
        {
            GridInt = gridInt;
            GridSector = gridSector;

            HasGridInt = GridInt != null;
            HasGridSector = GridSector != null;
        }
    }
}