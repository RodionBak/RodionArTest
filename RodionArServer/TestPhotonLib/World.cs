using System.Collections.Generic;
using System.Threading;
using ExitGames.Threading;

namespace TestPhotonLib
{
    public class World
    {
        public static readonly World Instance = new World();

        public List<UnityClient> Clients { get; private set; }

        //упрощенные юниты
        public List<GameUnit> units;

        private readonly ReaderWriterLockSlim readWriteLock;

        public World()
        {
            Clients = new List<UnityClient>();
            readWriteLock = new ReaderWriterLockSlim();
            units = new List<GameUnit>();

            //по умоланию, сразу есть только 2 юнита
            JoinNewUnit("Unit_1");
            JoinNewUnit("Unit_2");
        }


        //добавление юнита
        public void JoinNewUnit(string _characterName)
        {
            GameUnit unit1 = new GameUnit();
            unit1.characterName = _characterName;
            unit1.health = 100f;
            unit1.bodyState = GameUnit.BodyState.Alive;
            units.Add(unit1);
        }

        public UnityClient TryGetByName(string name)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                return Clients.Find(n => n.CharacterName.Equals(name));
            }
        }

        public bool IsContain(string name)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                return Clients.Exists(n => n.CharacterName.Equals(name));
            }
        }

        public void AddClient(UnityClient client)
        {
            using (WriteLock.TryEnter(this.readWriteLock, 1000))
            {
                Clients.Add(client);
            }
        }

        public void RemoveClient(UnityClient client)
        {
            using (WriteLock.TryEnter(this.readWriteLock, 1000))
            {
                Clients.Remove(client);
            }
        }

        ~World()
        {
            readWriteLock.Dispose();
        }
    }
}
