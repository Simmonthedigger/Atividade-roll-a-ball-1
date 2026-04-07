using UnityEngine;
using System.Collections; // OBRIGATÓRIO para IEnumerator

public class SplashSequence : MonoBehaviour
{
    private void Start()
    {
        // Iniciamos a contagem assim que a cena carrega
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        // Espera 2 segundos reais
        yield return new WaitForSeconds(2f);

        // Avisa o Manager para mudar o estado e carregar a próxima cena
        GameManager.Instance.ChangeState(GameState.MenuPrincipal);
        GameManager.Instance.LoadScene("MenuPrincipal");
    }
}