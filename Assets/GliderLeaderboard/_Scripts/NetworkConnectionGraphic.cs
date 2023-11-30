using UnityEngine;
using UnityEngine.UI;

namespace GliderServices
{
    public class NetworkConnectionGraphic : MonoBehaviour
    {
        public bool connected;
        [SerializeField] Sprite isConnectedGraphic;
        [SerializeField] Sprite notConnectedGraphic;
        [SerializeField] Image connectionImage;

        int timer;

        private void Start() 
        {
            connectionImage.sprite = notConnectedGraphic;
        }

        private void FixedUpdate() 
        {
            timer ++;
            if (timer % 49 == 0) UpdateConnected();
        }

        private void UpdateConnected()
        {
            connected = ServiceConnection.IsConnectedToNetwork();
            connectionImage.sprite = connected ? isConnectedGraphic : notConnectedGraphic;
        }
    }
}
