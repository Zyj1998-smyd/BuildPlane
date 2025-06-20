using GamePlay.Main;
using TMPro;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage
{
    public class OpenWelcomePageUi:InternalPageScript
    {
        private TextMeshProUGUI _txtName;
        private TMP_InputField _inputFieldName;
        private Button _btnRandomName;
        private Button _btnConfirm;

        public override void Initial()
        {
            // _txtName = transform.Find();
            
            base.Initial();
        }
    }
}