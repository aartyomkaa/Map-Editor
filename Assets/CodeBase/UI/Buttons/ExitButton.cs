using UnityEngine.SceneManagement;

namespace CodeBase.UI.Buttons
{
    public class ExitButton : BaseButton
    {
        protected override void OnClick()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}