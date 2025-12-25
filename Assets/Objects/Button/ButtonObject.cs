using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ButtonObject : ResizableMesh
{
    private const float outlineSize = 0.05f;
    private const float rotateInterval = 0.1f;
    private const float rotateForce = 0f;
    private const float commingBackForce = 0.2f;
    private const float selectedForce = 100;
    private const float clickResizeDuration = 0.1f;
    private readonly AnimationCurve clickResizeCurve = new(new Keyframe[] { new(0, 1.2f, 0.55f, 0.55f, 0, 0.2f), new(1f, 1f, 0.55f, 0.55f, 0.2f, 0f) });

    [SerializeField] private ResizableMesh outlinePrefab;

    private new Rigidbody rigidbody;
    private List<(Color color, ResizableMesh mesh)> outlines = new List<(Color, ResizableMesh)>();

    private void Awake()
    {
        if (Application.isPlaying)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(Rotate());
        }

        IEnumerator Rotate()
        {
            while (true)
            {
                var torqueDir = Vector3.zero;

                for (int i = 0; i < 3; i++)
                    torqueDir[i] = Random.Range(-1f, 1f);

                if (transform.eulerAngles != Vector3.zero)
                    rigidbody.AddTorque((-transform.eulerAngles).normalized * commingBackForce);

                rigidbody.AddTorque(torqueDir.normalized * rotateForce);

                yield return new WaitForSeconds(rotateInterval);
            }
        }
    }

    public void Select(Vector2Int from)
    {
        Deselect();

        if (from.y == 1)
            rigidbody.AddTorque(-transform.right * selectedForce);
        if (from.y == -1)
            rigidbody.AddTorque(transform.right * selectedForce);
        if (from.x == -1)
            rigidbody.AddTorque(transform.up * selectedForce);
        if (from.x == 1)
            rigidbody.AddTorque(-transform.up * selectedForce);

        SpawnOutline(Color.orange);
    }

    public void Deselect()
    {
        RemoveOutlines();
    }

    public void Click()
    {
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            var timer = 0f;
            while (timer < clickResizeDuration)
            {
                transform.localScale = Vector3.one * clickResizeCurve.Evaluate(timer / clickResizeDuration);

                timer += BoardTime.DeltaTime;
                yield return 0;
            }

            transform.localScale = Vector3.one;
        }
    }

    private void SpawnOutline(Color color)
    {
        var newOutline = Instantiate(outlinePrefab, transform);
        outlines.Add((color, newOutline));
        newOutline.SnapResize(Size + (outlines.Count + 1) * outlineSize * Vector3.one);
        var renderer = newOutline.GetComponent<MeshRenderer>();
        renderer.material.color = color;
    }

    private void RemoveOutline(Color color)
    {
        var outline = outlines.Single(x => x.color == color);
        outlines = outlines.Where(x => x.color != color).ToList();
        Destroy(outline.mesh.gameObject);
    }

    private void RemoveOutlines()
    {
        foreach (var (_, mesh) in outlines)
            Destroy(mesh.gameObject);

        outlines.Clear();
    }
}
