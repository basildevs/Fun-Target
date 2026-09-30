using UnityEngine;

namespace FunTarget
{
    [CreateAssetMenu(fileName = "FunTargetData", menuName = "ScriptableObjects/FunTargetData")]
    public class FunTargetData : ScriptableObject
    {
        public static FunTargetData instance;
        public static FunTargetData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<FunTargetData>("FunTargetData");
                }
                return instance;

            }
        }

        public string userName;
        public string accessToken;
        public string credit;
        public string gameId;
        public string url2;
        public string status;
        public string port;
        public string session;
    }

}
