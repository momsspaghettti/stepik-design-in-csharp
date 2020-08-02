namespace Inheritance.MapObjects {
    public interface IHasOwner {
        int Owner { get; set; }
    }

    public interface IEnemy {
        Army Army { get; set; }
    }

    public interface ITreasure {
        Treasure Treasure { get; set; }
    }

    public class Dwelling : IHasOwner {
        public int Owner { get; set; }
    }

    public class Mine : IHasOwner, IEnemy, ITreasure {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IEnemy, ITreasure {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolfs : IEnemy {
        public Army Army { get; set; }
    }

    public class ResourcePile : ITreasure {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction {
        public static void Make(Player player, object mapObject) {
            if (mapObject is IEnemy) {
                if (!player.CanBeat(((IEnemy) mapObject).Army)) {
                    player.Die();
                    return;
                }
            }
            
            if (mapObject is IHasOwner) {
                ((IHasOwner) mapObject).Owner = player.Id;
            }

            if (mapObject is ITreasure) {
                player.Consume(((ITreasure) mapObject).Treasure);
            }
        }
    }
}