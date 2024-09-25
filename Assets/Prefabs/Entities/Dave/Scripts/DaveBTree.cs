public class DaveBTree : BTree
{
    public EntityBase entity;
    protected override Node SetupTree()
    {
        root = new Selector("Root");
        root.Attach(new Node("Attack",
            () =>
            {
                return NodeState.SUCCESS;
            },
            () =>
            {
                entity.CurrentWeapon.PerformAttack(Attack.Type.Light);
                return NodeState.RUNNING;
            }));
        return root;
    }
}
