using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneLevelController : MonoBehaviour
{
    public Transform zoneLevelMidPoint;
    public float spawnDistance = 5f;
    public Transform levelOrientation;
    public Vector2 screenSize;
    public Camera cam;
    public Material holyWallMat;
    public Transform player;

    private ZoneModeController playerZoneController;
    private ObjectCollector collector;

    public static ZoneLevelController instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void StartZoneModeLevel(ZoneLevelScriptable levelData, Transform player)
    {
        this.player = player;
        playerZoneController = player.GetComponent<ZoneModeController>();
        collector = player.GetComponent<ObjectCollector>();
        SaveGameManager.instance.inZone = true;
        screenSize = new Vector2(Screen.width, Screen.height);
        StartCoroutine(ZoneLevelCoroutine(levelData));
    }

    IEnumerator ZoneLevelCoroutine(ZoneLevelScriptable levelData)
    {
        List<ZoneLevelObstacle> obstacles = new List<ZoneLevelObstacle>(levelData.obstacles);
        List<ZoneLevelMelonSpawn> melons = new List<ZoneLevelMelonSpawn>(levelData.melons);
        float t = 0;

        //testing the level
        t = levelData.START_LEVEL_AT_SECONDS_TESTER * levelData.levelLengthInSeconds;
        AudioManager.instance.SkipBGMTo(levelData.START_LEVEL_AT_SECONDS_TESTER);
        //testing the level END

        while (t < levelData.levelLengthInSeconds)
        {
            if (obstacles.Count > 0)
            {
                if (t >= obstacles[0].obstacleTime - obstacles[0].obstacleTimeToReachPlayer)
                {
                    StartCoroutine(ObstacleCoroutine(obstacles[0]));
                    obstacles.RemoveAt(0);
                }
            } 

            if (melons.Count > 0)
            {
                if (t >= melons[0].melonTime - melons[0].melonTimeToReachPlayer)
                {
                    print("spawning melon");
                    StartCoroutine(MelonCoroutine(melons[0]));
                    melons.RemoveAt(0);
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        LoadNextScene(levelData.nextLevelSceneName);
    }

    void LoadNextScene(string levelName)
    {
        SaveGameManager.instance.currentLevel++;
        SceneManager.LoadScene(levelName);
    }


    // Update the instantiated objects:

    IEnumerator ObstacleCoroutine(ZoneLevelObstacle obstacle)
    {
        float playerDistFromCamera = (cam.transform.position - zoneLevelMidPoint.position).magnitude;
        Vector3 offset = cam.ScreenToWorldPoint(new Vector3(screenSize.x * 0.5f, screenSize.y * 0.5f, playerDistFromCamera))
            - cam.ScreenToWorldPoint(new Vector3(screenSize.x * obstacle.obstacleScreenPos_X, screenSize.y * obstacle.obstacleScreenPos_y, playerDistFromCamera));

        Vector3 startpos = zoneLevelMidPoint.position + (levelOrientation.forward * spawnDistance) + offset;
        Vector3 endpos = zoneLevelMidPoint.position + offset;

        GameObject clone = Instantiate(obstacle.obstaclePrefab, startpos, Quaternion.LookRotation(levelOrientation.up));
        CreateMesh(clone, playerDistFromCamera);
        var m = clone.GetComponent<MeshRenderer>();
        var m2 = clone.transform.GetChild(0).GetComponent<MeshRenderer>();
        m.material = holyWallMat;
        m2.material = holyWallMat;
        m.material.color = obstacle.wallColor;
        m2.material.color = obstacle.wallColor;

        float t = 0;

        while (t < obstacle.obstacleTimeToReachPlayer)
        {
            clone.transform.position = Vector3.Lerp(startpos + offset, zoneLevelMidPoint.position + offset, t / obstacle.obstacleTimeToReachPlayer);
            t += Time.deltaTime;
            yield return null;
        }

        if (DetermineIfPoseMatches(obstacle))
        {
            float distance = new Vector2(player.transform.position.x - clone.transform.position.x, player.transform.position.y - clone.transform.position.y).magnitude;
            if (distance < 0.7f)
            {
                float perc = distance / 0.7f;
                int amount = (int)Mathf.Lerp(10, 1, perc);
                collector.CollectMelon(player.position + Vector3.up * 0.7f, amount);
            }
        }

        Destroy(clone);
    }

    public Transform tester1, tester2;

    IEnumerator MelonCoroutine(ZoneLevelMelonSpawn melon)
    {
        float playerDistFromCamera = (cam.transform.position - zoneLevelMidPoint.position).magnitude;
        Vector3 offset = cam.ScreenToWorldPoint(new Vector3(screenSize.x * 0.5f, screenSize.y * 0.5f, playerDistFromCamera)) 
            - cam.ScreenToWorldPoint(new Vector3(screenSize.x * melon.melonScreenPos_X, screenSize.y * melon.melonScreenPos_y, playerDistFromCamera));

        Vector3 startpos = zoneLevelMidPoint.position + (levelOrientation.forward * spawnDistance) + offset;
        Vector3 endpos = zoneLevelMidPoint.position + offset;

        var clone = Instantiate(melon.melonPrefab, startpos, Quaternion.LookRotation(levelOrientation.forward));
        float t = 0;

        while (t < melon.melonTimeToReachPlayer)
        {
            if (clone != null)
            {
                clone.transform.position = Vector3.Lerp(startpos + offset, zoneLevelMidPoint.position + offset, t / melon.melonTimeToReachPlayer);
            }
            t += Time.deltaTime;
            yield return null;
        }
        /*t = 0;
        while (t < 1)
        {
            
        }*/

        if (clone != null)
        {
            print("destroying melon");
            Destroy(clone);
        }
    }


    Mesh CreateMesh(GameObject wallHole, float playerDistFromCamera)
    {
        MeshFilter meshFilter = wallHole.transform.GetChild(0).GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        var bounds = wallHole.GetComponent<MeshRenderer>().bounds;

        Vector3 botLeftPoint = cam.ScreenToWorldPoint(new Vector3(0, 0, playerDistFromCamera)) + (levelOrientation.forward * spawnDistance);
        Vector3 topRightPoint = cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, playerDistFromCamera)) + (levelOrientation.forward * spawnDistance);
        float posZ = bounds.min.z;

        Vector3[] vertices = new[] {
            // creating vertices of quad. aligning them in shape of square
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, topRightPoint.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, topRightPoint.y, posZ)),

            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.max.y, posZ)),

            wallHole.transform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.max.y, posZ)),

            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, botLeftPoint.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, botLeftPoint.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.min.y, posZ)),
        };
        mesh.vertices = vertices;


        // generate uv
        Vector2[] uv = new[] {
            // generate uv for corresponding vertices also in form of square
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
        };
        mesh.uv = uv;
        Vector3[] normals = new[] {
            // normals same as tris
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
        };
        mesh.normals = normals;
        int[] triangles = new[] {
            0,1,2,// first tris
            2,1,3,// second tris

            4,5,6,
            6,5,7,

            8,9,10,
            10,9,11,

            12,13,14,
            14,13,15
        };


        mesh.triangles = triangles;
        meshFilter.mesh = mesh;

        return mesh;
    }

    bool DetermineIfPoseMatches(ZoneLevelObstacle o)
    {
        switch (o.obstaclePrefab.name)
        {
            case "holyWall_up": if (playerZoneController.playerCurrentPose == ZoneModePoses.UP) return true; break;
            case "holyWall_down": if (playerZoneController.playerCurrentPose == ZoneModePoses.DOWN) return true; break;
            case "holyWall_left": if (playerZoneController.playerCurrentPose == ZoneModePoses.LEFT) return true; break;
            case "holyWall_right": if (playerZoneController.playerCurrentPose == ZoneModePoses.RIGHT) return true; break;
            case "holyWall_mid": if (playerZoneController.playerCurrentPose == ZoneModePoses.MID) return true; break;
            case "holyWall_upRight": if (playerZoneController.playerCurrentPose == ZoneModePoses.UP_RIGHT) return true; break;
            case "holyWall_upLeft": if (playerZoneController.playerCurrentPose == ZoneModePoses.UP_LEFT) return true; break;
            case "holyWall_downRight": if (playerZoneController.playerCurrentPose == ZoneModePoses.DOWN_RIGHT) return true; break;
            case "holyWall_downLeft": if (playerZoneController.playerCurrentPose == ZoneModePoses.DOWN_LEFT) return true; break;
        }
        return false;
    }
}
