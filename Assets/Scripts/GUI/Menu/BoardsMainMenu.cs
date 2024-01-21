
public class BoardsMainMenu : BoardsScore
{
    private void Start()
    {
        SetBoards();
        Inventory.Inst.EventLoad += SetBoards;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(Inventory.Inst != null)
            Inventory.Inst.EventLoad -= SetBoards;
    }

    private void SetBoards() => BoardsBind(Inventory.Inst.TotalScore);

}
