using IOSEF;
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

    public IProperty<float> FloatReadOnly => _float_ro;
    public IProperty<int> IntegerReadOnly => _integer_ro;
    public IProperty<uint> UIntegerReadOnly => _uinteger_ro;
    public IProperty<long> LongReadOnly => _long_ro;
    public IProperty<ulong> ULongReadOnly => _ulong_ro;
    public IProperty<Vector2> Vector2ReadOnly => _vector2_ro;
    public IProperty<Vector2Int> Vector2IntReadOnly => _vector2int_ro;
    public IProperty<Vector3> Vector3ReadOnly => _vector3_ro;
    public IProperty<Vector3Int> Vector3IntReadOnly => _vector3int_ro;
    public IProperty<bool> BoolReadOnly => _binarystatus_ro;
    public IProperty<string> StringFieldReadOnly => _stringfield_ro;

    private Property<float> _float_ro;
    private Property<int> _integer_ro;
    private Property<uint> _uinteger_ro;
    private Property<long> _long_ro;
    private Property<ulong> _ulong_ro;
    private Property<Vector2> _vector2_ro;
    private Property<Vector2Int> _vector2int_ro;
    private Property<Vector3> _vector3_ro;
    private Property<Vector3Int> _vector3int_ro;
    private Property<bool> _binarystatus_ro;
    private Property<bool> _toggleslide_ro;
    private Property<string> _stringfield_ro;

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

        _float_ro = new Property<float>();
        _integer_ro = new Property<int>();
        _uinteger_ro = new Property<uint>();
        _long_ro = new Property<long>();
        _ulong_ro = new Property<ulong>();
        _vector2_ro = new Property<Vector2>();
        _vector2int_ro = new Property<Vector2Int>();
        _vector3_ro = new Property<Vector3>();
        _vector3int_ro = new Property<Vector3Int>();
        _binarystatus_ro = new Property<bool>();
        _stringfield_ro = new Property<string>();
    }
}
