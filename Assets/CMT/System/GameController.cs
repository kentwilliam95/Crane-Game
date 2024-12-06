using Core;
using UnityEngine;

namespace CMT
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private GameUIController _uIController;
        [SerializeField] private AreaSpawner _areaSpawner;
        
        [Header("Audio")]
        [SerializeField] private AudioClip audioClipCelebration;
        [SerializeField] private AudioClip audioClipFail;
 
        private void Start()
        {
            PanelFade.Instance.Hide();
            TouchController.Instance.DisableInput();
            _uIController.OnHideMenuComplete = GameUIController_HideComplete;
            _playerController.GotAnItem = Player_OnGotAnItem;
            _playerController.NotGetAnyItem = Player_OnNotGetAnyItem;
            _uIController.OnHideGameResultComplete = GameUIController_BackToGame; 
            AudioManager.Instance.SetMusicVolume(1f);
        }

        private void GameUIController_HideComplete()
        {
            TouchController.Instance.EnableInput();
            AudioManager.Instance.SetMusicVolume(0.5f);
        }

        private void GameUIController_BackToGame()
        {
            TouchController.Instance.EnableInput();
        }
        
        private void Player_OnNotGetAnyItem()
        {
            AudioManager.Instance.PlaySfx(audioClipFail);
        }

        private void Player_OnGotAnItem(Pickable item)
        {
            TouchController.Instance.DisableInput();
            _uIController.ShowResult(item);
            AudioManager.Instance.PlaySfx(audioClipCelebration, 1.5f);
            ObjectPool.Instance.UnSpawn(item.gameObject);
        }
    }
}