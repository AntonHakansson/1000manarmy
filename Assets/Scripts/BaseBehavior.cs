using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{

/// <summary>
///     Custom MonoBehavior, should be used consistently throughout the game
/// </summary>
public class BaseBehavior : MonoBehaviour {

    /*----------  Cache frequently used components  ----------*/
    [HideInInspector] private Animation _animation;

    [HideInInspector] private AudioSource _audio;

    [HideInInspector] private Camera _camera;

    [HideInInspector] private Collider _collider;

    [HideInInspector] private Collider2D _collider2D;

    [HideInInspector] private ConstantForce _constantForce;

    [HideInInspector] private HingeJoint _hingeJoint;

    [HideInInspector] private Light _light;

    [HideInInspector] private ParticleSystem _particleSystem;

    [HideInInspector] private Renderer _renderer;

    [HideInInspector] private Material _material;

    [HideInInspector] private Mesh _mesh;

    [HideInInspector] private MeshFilter _meshFilter;

    [HideInInspector] private Rigidbody _rigidbody;

    [HideInInspector] private Rigidbody2D _rigidbody2D;

    /// <summary> Gets the Animation attached to the object. </summary>
    public Animation GetAnimation
    {
        get { return _animation ? _animation : (_animation = GetComponent<Animation>()); }
    }

    /// <summary> Gets the AudioSource attached to the object. </summary>
    public AudioSource GetAudio
    {
        get { return _audio ? _audio : (_audio = GetComponent<AudioSource>()); }
    }

    /// <summary> Gets the Camera attached to the object. </summary>
    public Camera GetCamera
    {
        get { return _camera ? _camera : (_camera = GetComponent<Camera>()); }
    }

    /// <summary> Gets the Collider attached to the object. </summary>
    public Collider GetCollider
    {
        get { return _collider ? _collider : (_collider = GetComponent<Collider>()); }
    }

    /// <summary> Gets the Collider2D attached to the object. </summary>
    public Collider2D GetCollider2D
    {
        get { return _collider2D ? _collider2D : (_collider2D = GetComponent<Collider2D>()); }
    }

    /// <summary> Gets the ConstantForce attached to the object. </summary>
    public ConstantForce GetConstantForce
    {
        get { return _constantForce ? _constantForce : (_constantForce = GetComponent<ConstantForce>()); }
    }

    /// <summary> Gets the HingeJoint attached to the object. </summary>
    public HingeJoint GetHingeJoint
    {
        get { return _hingeJoint ? _hingeJoint : (_hingeJoint = GetComponent<HingeJoint>()); }
    }

    /// <summary> Gets the Light attached to the object.  </summary>
    public Light GetLight
    {
        get { return _light ? _light : (_light = GetComponent<Light>()); }
    }

    /// <summary> Gets the ParticleSystem attached to the object </summary>
    public ParticleSystem GetParticleSystem
    {
        get { return _particleSystem ? _particleSystem : (_particleSystem = GetComponent<ParticleSystem>()); }
    }

    /// <summary>  Gets the Renderer attached to the object.  </summary>
    public Renderer GetRenderer
    {
        get { return _renderer ? _renderer : (_renderer = GetComponent<Renderer>()); }
    }

    /// <summary>  Gets the Material attached to the object.  </summary>
    public Material GetMaterial
    {
        get { return _material ? _material : (_material = GetRenderer.material); }
    }

    /// <summary>  Gets the Mesh attached to the object.  </summary>
    public Mesh GetMesh
    {
        get { return _mesh ? _mesh : (_mesh = GetComponent<Mesh>()); }
    }

    /// <summary>  Gets the MeshFilter attached to the object.  </summary>
    public MeshFilter GetMeshFilter
    {
        get { return _meshFilter ? _meshFilter : (_meshFilter = GetComponent<MeshFilter>()); }
    }

    /// <summary>  Gets the Rigidbody attached to the object. </summary>
    public Rigidbody GetRigidbody
    {
        get { return _rigidbody ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>()); }
    }

    /// <summary>  Gets the Rigidbody2D attached to the object.  </summary>
    public Rigidbody2D GetRigidbody2D
    {
        get { return _rigidbody2D ? _rigidbody2D : (_rigidbody2D = GetComponent<Rigidbody2D>()); }
    }
}

}