using OC;
using UnityEngine;

public class GenericPanelTestInteraction : MonoBehaviour, IInteractable
{
    public Property<float> Float { get; set; }
    public Property<int> Integer { get; set; }
    public Property<uint> UInteger  { get; set; }
    public Property<long> Long  { get; set; }
    public Property<ulong> ULong  { get; set; }
    public Property<Vector2> Vector2  { get; set; }
    public Property<Vector2Int> Vector2Int  { get; set; }
    public Property<Vector3> Vector3  { get; set; }
    public Property<Vector3Int> Vector3Int  { get; set; }
    public Property<bool> BinaryStatus  { get; set; }
    public Property<bool> ToggleSlide  { get; set; }
    public Property<string> StringField  { get; set; }

    public IProperty<float> FloatReadOnly => _floatRO;
    public IProperty<int> IntegerReadOnly => _integerRO;
    public IProperty<uint> UIntegerReadOnly => _uintegerRO;
    public IProperty<long> LongReadOnly => _longRO;
    public IProperty<ulong> ULongReadOnly => _ulongRO;
    public IProperty<Vector2> Vector2ReadOnly => _vector2RO;
    public IProperty<Vector2Int> Vector2IntReadOnly => _vector2IntRO;
    public IProperty<Vector3> Vector3ReadOnly => _vector3RO;
    public IProperty<Vector3Int> Vector3IntReadOnly => _vector3IntRO;
    public IProperty<bool> BoolReadOnly => _binarystatusRO;
    public IProperty<string> StringFieldReadOnly => _stringfieldRO;

    private Property<float> _floatRO;
    private Property<int> _integerRO;
    private Property<uint> _uintegerRO;
    private Property<long> _longRO;
    private Property<ulong> _ulongRO;
    private Property<Vector2> _vector2RO;
    private Property<Vector2Int> _vector2IntRO;
    private Property<Vector3> _vector3RO;
    private Property<Vector3Int> _vector3IntRO;
    private Property<bool> _binarystatusRO;
    private Property<bool> _toggleslideRO;
    private Property<string> _stringfieldRO;

    private void Start()
    {
        Float = new Property<float>();
        Integer = new Property<int>();
        UInteger = new Property<uint>();
        Long = new Property<long>();
        ULong = new Property<ulong>();
        Vector2 = new Property<Vector2>();
        Vector2Int = new Property<Vector2Int>();
        Vector3 = new Property<Vector3>();
        Vector3Int = new Property<Vector3Int>();
        BinaryStatus = new Property<bool>();
        ToggleSlide = new Property<bool>();
        StringField = new Property<string>();

        _floatRO = new Property<float>();
        _integerRO = new Property<int>();
        _uintegerRO = new Property<uint>();
        _longRO = new Property<long>();
        _ulongRO = new Property<ulong>();
        _vector2RO = new Property<Vector2>();
        _vector2IntRO = new Property<Vector2Int>();
        _vector3RO = new Property<Vector3>();
        _vector3IntRO = new Property<Vector3Int>();
        _binarystatusRO = new Property<bool>();
        _stringfieldRO = new Property<string>();
    }
}
