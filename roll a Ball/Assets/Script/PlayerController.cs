using UnityEngine;
using UnityEngine.InputSystem;

// Controlador para jogo tipo Roll-a-Ball usando o novo Input System.
// Instruções rápidas:
// 1) Adicione este script no GameObject do jogador (a esfera).
// 2) Certifique-se de que o GameObject tem um Rigidbody (use Freeze Rotation em X/Y/Z se quiser evitar que a bola tombe).
// 3) No inspector, arraste a ação 'Move' do seu asset InputSystem_Actions (tipo Vector2) para o campo "Move Action".
// 4) Ajuste 'speed' e 'maxSpeed' conforme necessário.

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Referência à ação 'Move' (Vector2) criada no seu Input Actions asset")]
    public InputActionReference moveAction;

    [Header("Movement")]
    [Tooltip("Força aplicada por unidade de input. Use valores como 5-20 dependendo da massa do Rigidbody.")]
    public float speed = 10f;
    [Tooltip("Velocidade máxima horizontal (m/s)")]
    public float maxSpeed = 5f;
    [Tooltip("Se verdadeiro usa AddForce, senão define diretamente a velocidade horizontal")]
    public bool useForce = true;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody não encontrado no GameObject. Adicione um Rigidbody.");
        }
    }

    void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 input = Vector2.zero;
        if (moveAction != null && moveAction.action != null)
        {
            input = moveAction.action.ReadValue<Vector2>();
        }

        // Input Vector2: x -> esquerda/direita, y -> frente/tras
        Vector3 desired = new Vector3(input.x, 0f, input.y);

        if (useForce)
        {
            // Aplica força contínua baseada no input
            rb.AddForce(desired * speed, ForceMode.Force);

            // Limita velocidade horizontal para evitar aceleração infinita
            Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVel.magnitude > maxSpeed)
            {
                Vector3 clamped = horizontalVel.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
            }
        }
        else
        {
            // Define diretamente a velocidade horizontal (mais responsivo, ignora física de aceleração)
            Vector3 targetVel = desired * maxSpeed;
            rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
        }
    }
}

