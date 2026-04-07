using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    [Header("Configurações de Estado")]
    [SerializeField] private GameState currentState;

    private void Awake()
    {
        // Lógica de Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicia o fluxo a partir da cena de Boot
        ChangeState(GameState.Iniciando);
        LoadScene("Splash");
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"[GameManager] Estado alterado para: {currentState}");
    }

    // Único ponto de acesso para carregar cenas
    public void LoadScene(string sceneName)
    {
        // Validação de estado simples
        if (sceneName == "SampleScene" && currentState != GameState.MenuPrincipal)
        {
            Debug.LogWarning("Mudança de cena negada: Só podemos ir para a Gameplay a partir do Menu!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // Alocação de Input (Input System)
    public void AssignPlayerInput(PlayerInput playerInput)
    {
        if (playerInput != null)
        {
            Debug.Log("Input alocado ao jogador com sucesso.");
            // Aqui você poderia configurar esquemas de controle específicos
        }
    }
}