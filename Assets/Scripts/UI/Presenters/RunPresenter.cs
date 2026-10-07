public class RunPresenter
{
    private readonly RunScreen _screen;

    public RunPresenter(RunScreen screen)
    {
        _screen = screen;
    }

    public void HandleStateEntered()
    {
        _screen.Show();
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }
}