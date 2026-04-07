using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // As funções DEVEM ser public para aparecerem no botão
    public void BotaoIniciar()
    {
        // 1º: MUDAR O ESTADO (Isso autoriza o GameManager)
        GameManager.Instance.ChangeState(GameState.MenuPrincipal);

        // 2º: DEPOIS PEDIR PARA CARREGAR A CENA
        GameManager.Instance.LoadScene("SampleScene");
    }

    public void BotaoSair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}