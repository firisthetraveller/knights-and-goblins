using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AudioSource))]
public class AutoFootSteps : MonoBehaviour
{
    public float Volume = .33f;
    /// <summary>
    /// Min & Max volume Variance
    /// </summary>
    public Vector2 RandomizedVolumeRange = new Vector2(.95f, 1.05f);
    /// <summary>
    /// Min and Max Pitch Variance
    /// </summary>
    public Vector2 RandomizedPitchRange = new Vector2(.95f, 1.05f);

    /// <summary>
    /// The Distance the floor is expected to be down at, from this component.
    /// </summary>
    public float FloorDistance = .55f;

    /// <summary>
    /// The distance required to travel before playing a step.
    /// </summary>
    public float DistancePerStep = 1.7f;

    private AudioSource _audioSource;
    /// <summary>
    /// The sounds that'll be played on step.
    /// </summary>
    public FootProfile CurrentFootProfile;
    /// <summary>
    /// The Type of Material the player is standing on, by name. (Rock, Grass, etc - defined in the supplied FootProfile.)
    /// </summary>
    [HideInInspector] public string CurrentFootMaterialName;
    [HideInInspector] public TerrainCollider CurrentTerrain;
    [HideInInspector] public float[,,] CurrentTerrainAlphas;

    /// <summary>
    /// The speed that the player currently crouches at.
    /// </summary>
    public float CrouchSpeed = 2.5f;
    float _walkThreshhold => Mathf.Lerp(CrouchSpeed, WalkSpeed, .5f);
    /// <summary>
    /// The speed that the player currently walks at.
    /// </summary>
    public float WalkSpeed = 5;
    float _runThreshhold => Mathf.Lerp(WalkSpeed, RunSpeed, .5f);
    /// <summary>
    /// The speed that the player currently runs at.
    /// </summary>
    public float RunSpeed = 10;

    public Tilemap[] tilemaps;

    void Start() => _audioSource = GetComponent<AudioSource>();

    private Vector3[] _previousPositions = new Vector3[6];
    private void insertPosition(Vector3 newPosition)
    {
        for (int i = 5; i >= 1; i--)
            _previousPositions[i] = _previousPositions[i - 1];
        _previousPositions[0] = newPosition;
    }

    private float _timeSinceLastStep;
    private float _preStepTravelledDistance;
    void LateUpdate()
    {
        // Only run every .125s
        _timeSinceLastStep += Time.deltaTime;
        if (_timeSinceLastStep < .125f)
            return;
        _timeSinceLastStep = 0;

        // add the distance travelled
        float startY = _previousPositions[0].y;
        _previousPositions[0].y = _previousPositions[1].y;
        _preStepTravelledDistance += Vector2.Distance(_previousPositions[0], _previousPositions[1]);
        _previousPositions[0].y = startY;
        insertPosition(transform.position);

        Vector3 _currentVelocity = (_previousPositions[0] - _previousPositions[1]) / .125f;

        FootMaterialSpec curSpec = null;

        // Footsteps
        if (_preStepTravelledDistance > DistancePerStep)
        {
            _preStepTravelledDistance = 0;
            refreshMaterial();

            curSpec ??= CurrentFootProfile.MaterialSpecifications.FirstOrDefault(spec => spec.MaterialName == CurrentFootMaterialName);
            if (curSpec != null)
            {
                float randomize = Random.Range(RandomizedVolumeRange.x, RandomizedVolumeRange.y);
                _audioSource.volume = Volume * randomize * curSpec.VolumeMultiplier;
                if (_currentVelocity.magnitude < _walkThreshhold)
                {
                    _audioSource.volume *= .80f;
                    playRandomOrdered(curSpec.SoftSteps);
                }
                else if (_currentVelocity.magnitude < _runThreshhold)
                {
                    playRandomOrdered(curSpec.MediumSteps);
                }
                else
                {
                    playRandomOrdered(curSpec.HardSteps);
                }
            }
        }
    }

    private bool getTileOnCurrentPosition(out string name)
    {
        Vector3Int playerPosition = Vector3Int.FloorToInt(transform.position);
        //tilemaps.Where(t => t.cellBounds)

        foreach (Tilemap tilemap in tilemaps)
        {
            // Check if the player's position is within the bounds of the current tilemap
            // if (tilemap.cellBounds.Contains(playerPosition))
            {
                // Get the tile at the player's position
                Vector3Int playerTilePosition = tilemap.WorldToCell(playerPosition);
                TileBase tile = tilemap.GetTile(playerTilePosition);

                // Do something with the tile (e.g., check its properties, react accordingly) then exit
                if (tile != null)
                {
                    // Debug.Log($"Tile found: {tile.name}, at: {playerTilePosition}");
                    name = tile.name;
                    return true;
                    // You can perform further actions based on the tile found
                }
            }
        }
        // Debug.Log($"No tile found at player's position on this tilemap");
        name = "Air";
        return false;
    }

    private void refreshMaterial()
    {
        if (!getTileOnCurrentPosition(out string name))
        {
            CurrentFootMaterialName = "Air";
        }
        else
        {
            CurrentFootMaterialName = getMaterialName(name);
        }
    }

    private string getMaterialName(string tileName)
    {
        foreach (FootMaterialSpec footMat in CurrentFootProfile.MaterialSpecifications)
            foreach (string similarName in footMat.SimilarNames)
                if (tileName.Contains(similarName, System.StringComparison.CurrentCultureIgnoreCase))
                    return footMat.MaterialName;
        return null;
    }

    private uint _stepCount = 0;
    private void playRandomOrdered(AudioClip[] clips, float scaleVol = 1)
    {

        if (clips == null || clips.Length == 0)
            return;
        _audioSource.pitch = UnityEngine.Random.Range(RandomizedPitchRange.x, RandomizedPitchRange.y);
        _audioSource.PlayOneShot(clips[_stepCount++ % clips.Length], scaleVol);
        if (_stepCount % clips.Length == 0)
        {
            AudioClip[] shuffled = clips.Randomize().ToArray();
            if (clips[clips.Length - 1] == shuffled[0]) // Prevent sound playing twice edge case
            {
                shuffled[0] = shuffled[shuffled.Length - 1];
                shuffled[shuffled.Length - 1] = clips[clips.Length - 1];
            }
            for (int i = 0; i < clips.Length; i++)
                clips[i] = shuffled[i];
        }
    }
    private enum TerrainAction
    {
        Jump, Land, Walk, Run, Crouch, Slip
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3Int.FloorToInt(transform.position), 0.2f);
    }
}

public static class AutoFootStepExtensions
{
    private static System.Random rnd = new System.Random();
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        => source.OrderBy((item) => rnd.Next());
}