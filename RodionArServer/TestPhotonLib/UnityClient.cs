using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using TestPhotonLib.Common;

namespace TestPhotonLib
{
    public class UnityClient:ClientPeer
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public string CharacterName { get; private set; }

   


        //Клиент
        public UnityClient(InitRequest initRequest) : base(initRequest)
        {
            Log.Debug("Client connected");
            World.Instance.AddClient(this);
        }


        //найти клиента по имени
        public GameUnit GetUnitByName(string _characterName)
        {
            GameUnit result = null;

            foreach (GameUnit _unit in World.Instance.units)
            {
                if (_unit.characterName == _characterName)
                {
                    result = _unit;
                }
            }

            return result;
        }


        //событие смерти игрока
        public void OnUnitDeath(GameUnit _unit, SendParameters sendParameters)
        {
            //отправить событие
            var eventData2 = new EventData((byte)EventCode.Death);
            eventData2.Parameters = new Dictionary<byte, object>
                            {
                                {(byte)ParameterCode.CharacterName, _unit.characterName }
                            };
            eventData2.SendTo(World.Instance.Clients, sendParameters);
        }


        //событие урона игрока
        public void OnUnitDamage(GameUnit _unit, SendParameters sendParameters)
        {
            //отправить событие
            var eventData2 = new EventData((byte)EventCode.Damage);
            eventData2.Parameters = new Dictionary<byte, object>
                            {
                                {(byte)ParameterCode.CharacterName, _unit.characterName }
                            };
            eventData2.SendTo(World.Instance.Clients, sendParameters);
        }



        //событие обновления жизней игрока
        public void OnHealthUpdate(GameUnit _unit,  SendParameters sendParameters)
        {
            //отправить событие
            var eventData2 = new EventData((byte)EventCode.UpdateHealth);
            eventData2.Parameters = new Dictionary<byte, object>
                            {
                                {(byte)ParameterCode.Health, _unit.health},
                                {(byte)ParameterCode.CharacterName, _unit.characterName }
                            };
            eventData2.SendTo(World.Instance.Clients, sendParameters);
        }


        //событие воскрешения игрока
        public void OnRespawn(GameUnit _unit, SendParameters sendParameters)
        {
            //отправить событие
            var eventData2 = new EventData((byte)EventCode.Respawn);
            eventData2.Parameters = new Dictionary<byte, object>
                            {
                                {(byte)ParameterCode.CharacterName, _unit.characterName }
                            };
            eventData2.SendTo(World.Instance.Clients, sendParameters);
        }


        //ударить юнита
        public void HitUnit(GameUnit _unit, SendParameters sendParameters)
        {
            if (_unit.bodyState == GameUnit.BodyState.Alive)
            {
                _unit.health -= 10f;//по умолчанию 10
                Log.Info("Update unit health " + _unit.health.ToString());

                //игрок умер
                if (_unit.health <= 0f)
                {
                    _unit.health = 0f;
                    _unit.bodyState = GameUnit.BodyState.Death;

                    //событие смерти игрока
                    OnUnitDeath(_unit, sendParameters);

                }

                //отправить события
                OnUnitDamage(_unit, sendParameters);
                OnHealthUpdate(_unit, sendParameters);
            }
        }





        //запрос от клиента
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                //команда удара
                case (byte)OperationCode.Hit:
                    
                    //получить имя ударяемого юнита
                    Log.Info("Hit from client");

                    //если получили имя от клиента
                    string characterName = "";
                    if (operationRequest.Parameters[(byte) ParameterCode.CharacterName] != null)
                    {
                        characterName = operationRequest.Parameters[(byte)ParameterCode.CharacterName].ToString();
                        Log.Info("Get unit by name " + characterName);
                        GameUnit _unit = GetUnitByName(characterName);

                        //ударить игрока
                        if (_unit != null)
                        {
                            HitUnit(_unit, sendParameters);
                        }
                    }

                    break;

                
                default:
                    Log.Debug("Unknown OperationRequest received!:" + operationRequest.OperationCode);
                    break;
            }
        }


        //клиент отключился
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            World.Instance.RemoveClient(this);
            Log.Debug("Disconnected!");
        }



        
    }
}
