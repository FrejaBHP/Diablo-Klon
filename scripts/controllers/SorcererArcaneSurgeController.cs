using Godot;

public partial class SorcererArcaneSurgeController : Node {
    public Player PlayerOwner { get; private set; }

    private const double manaRequired = 5;
    private double manaSpent = 0;

    public void SetPlayerOwner(Player player) {
        PlayerOwner = player;
        PlayerOwner.ManaSpent += ProcessManaSpent;
    }

    private void ProcessManaSpent(double mana) {
        double manaAcc = manaSpent + mana;

        if (manaAcc >= manaRequired) {
            manaSpent = manaAcc % manaRequired;
            GiveBuff();
        }
        else {
            manaSpent = manaAcc;
        }
    }

    private void GiveBuff() {
        PlayerOwner.ReceiveEffect(new ArcaneSurgeEffect(PlayerOwner));
    }
}
