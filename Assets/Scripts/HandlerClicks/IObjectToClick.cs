namespace HandlerClicks
{
    public interface IObjectToClick
    {
        void Clicked();

        void OnStartDrag();
        void OnEndDrag();
    }
}