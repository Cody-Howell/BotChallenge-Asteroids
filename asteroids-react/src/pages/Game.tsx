import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import ReactAsteroids from '../components/AsteroidsDisplay';

export default function Game() {
    const { id } = useParams<{ id: string }>();
    const gameId = id ? parseInt(id) : null;
    const [playerId, setPlayerId] = useState<number | null>(null);
    const [keys, setKeys] = useState<Set<string>>(new Set());
    const [stars, setStars] = useState<{ x: number, y: number }[]>([]);
    const [ships, setShips] = useState<Ship[]>([]);
    const [bullets, setBullets] = useState<InternalGameObject[]>([]);
    const [asteroids, setAsteroids] = useState<Asteroid[]>([]);
    const [gameSize, setGameSize] = useState<{ width: number, height: number }>({ width: 1000, height: 1000 });
    const [scoreboard, setScoreboard] = useState<ScoreType[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [isRegistering, setIsRegistering] = useState(false);

    const registerPlayer = async () => {
        if (!gameId || isRegistering) return;

        setIsRegistering(true);
        try {
            const res = await fetch(`/api/game/register/${gameId}`, {
                method: "POST"
            });

            if (!res.ok) {
                throw new Error(`Failed to register: ${res.statusText}`);
            }

            const data = await res.json();
            setPlayerId(data.playerId);
            console.log(`Registered as player ${data.playerId} in game ${gameId}`);
        } catch (err: any) {
            console.error('Failed to register to game:', err);
            setError(`Failed to register to game: ${err.message}`);
        } finally {
            setIsRegistering(false);
        }
    };

    const setNewKeys = (keys: Set<string>) => {
        if (!gameId || !playerId) return;

        const moveArray = [];
        if (keys.has('w')) moveArray.push("ACCEL 1")
        if (keys.has('s')) moveArray.push("ACCEL -1")
        if (keys.has('d')) moveArray.push("TURN 1")
        if (keys.has('a')) moveArray.push("TURN -1")
        if (keys.has(' ')) moveArray.push("FIRE")
        if (keys.has('b')) moveArray.push("BRAKE")

        fetch(`/api/game/move/${gameId}/${playerId}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(moveArray)
        })
        setKeys(keys);
    }

    useEffect(() => {
        if (!gameId) {
            setError("Invalid game ID");
            return;
        }

        // Fetch game size from API
        fetch(`/api/game/size/${gameId}`)
            .then(res => {
                if (!res.ok) {
                    throw new Error(`Failed to fetch game size: ${res.statusText}`);
                }
                return res.json();
            })
            .then(data => {
                setGameSize(data);

                // Generate stars within game boundaries
                const stars = [];
                const numOfStars = 100;
                for (let i = 0; i < numOfStars; i++) {
                    stars.push({ x: Math.random() * data.width, y: Math.random() * data.height })
                }
                setStars(stars);
            })
            .catch(err => {
                console.error('Failed to fetch game size:', err);
                setError(`Failed to fetch game size: ${err.message}`);
            });

    }, [gameId]);

    useEffect(() => {
        if (!gameId || gameId < 0) return;

        // Fetch initial game state from debug endpoint
        fetch(`/api/game/debug/${gameId}`)
            .then(res => {
                if (!res.ok) {
                    throw new Error(`Failed to fetch initial game state: ${res.statusText}`);
                }
                return res.json();
            })
            .then(data => {
                const json = data as { Players: string[], Asteroids: string[], Bullets: string[] };
                const scores: ScoreType[] = [];
                const players: Ship[] = [];

                for (var player of json.Players) {
                    const items = player.split(' ');
                    const health = Number.parseInt(items[5]);
                    if (health > 0) {
                        players.push({
                            x: Number.parseFloat(items[1]),
                            y: Number.parseFloat(items[2]),
                            orientation: (Number.parseFloat(items[3]) + 90) % 360,
                            id: Number.parseInt(items[0])
                        });
                    }
                    scores.push({
                        id: Number.parseInt(items[0]),
                        score: Number.parseInt(items[8]),
                        health: health
                    })
                }
                setShips(players);

                scores.sort((a, b) => b.score - a.score);
                setScoreboard(scores);

                const bullets: InternalGameObject[] = [];
                for (var bullet of json.Bullets) {
                    const items = bullet.split(' ');
                    bullets.push({
                        x: Number.parseFloat(items[1]),
                        y: Number.parseFloat(items[2]),
                        orientation: (Number.parseFloat(items[3]) + 90) % 360
                    })
                }
                setBullets(bullets);

                const asteroids: Asteroid[] = [];
                for (var asteroid of json.Asteroids) {
                    const items = asteroid.split(' ');
                    asteroids.push({
                        x: Number.parseFloat(items[0]),
                        y: Number.parseFloat(items[1]),
                        orientation: (Number.parseFloat(items[2]) + 90) % 360,
                        radius: Number.parseFloat(items[4])
                    })
                }
                setAsteroids(asteroids);
            })
            .catch(err => {
                console.error('Failed to fetch initial game state:', err);
            });

        console.log(`Connecting to game ${gameId}`)
        const socket = new WebSocket(`ws://localhost:5038/api/game/ws/${gameId}`);

        socket.addEventListener("open", () => {
            console.log("Socket opened.")
        });

        socket.addEventListener("message", (event) => {
            console.log("Event data: ", event.data);
            try {
                const json = JSON.parse(event.data) as { Players: string[], Asteroids: string[], Bullets: string[] };
                const scores: ScoreType[] = [];
                const players: Ship[] = [];

                for (var player of json.Players) {
                    const items = player.split(' ');
                    const health = Number.parseInt(items[5]);
                    if (health > 0) {
                        players.push({
                            x: Number.parseFloat(items[1]),
                            y: Number.parseFloat(items[2]),
                            orientation: (Number.parseFloat(items[3]) + 90) % 360,
                            id: Number.parseInt(items[0])
                        });
                    }
                    scores.push({
                        id: Number.parseInt(items[0]),
                        score: Number.parseInt(items[8]),
                        health: health
                    })
                }
                setShips(players);

                // sort scores descending by score
                scores.sort((a, b) => b.score - a.score);
                setScoreboard(scores);

                const bullets: InternalGameObject[] = [];
                for (var bullet of json.Bullets) {
                    const items = bullet.split(' ');
                    bullets.push({
                        x: Number.parseFloat(items[1]),
                        y: Number.parseFloat(items[2]),
                        orientation: (Number.parseFloat(items[3]) + 90) % 360
                    })
                }
                setBullets(bullets);

                const asteroids: Asteroid[] = [];
                for (var asteroid of json.Asteroids) {
                    const items = asteroid.split(' ');
                    asteroids.push({
                        x: Number.parseFloat(items[0]),
                        y: Number.parseFloat(items[1]),
                        orientation: (Number.parseFloat(items[2]) + 90) % 360,
                        radius: Number.parseFloat(items[4])
                    })
                }
                setAsteroids(asteroids);
            } catch (err) {
                console.error("Error parsing game data:", err);
            }
        });

        socket.addEventListener("error", (event) => {
            console.error("WebSocket error:", event);
            setError("WebSocket connection error");
        });

        socket.addEventListener("close", () => {
            console.log("Socket closed.");
        });

        return () => {
            if (socket.readyState === WebSocket.OPEN || socket.readyState === WebSocket.CONNECTING) {
                socket.close();
            }
        };
    }, [gameId]);

    if (error) {
        return (
            <div style={{ padding: '2rem', textAlign: 'center' }}>
                <h1>Error</h1>
                <p>{error}</p>
            </div>
        );
    }

    return (
        <div style={{ position: 'relative', width: '100%', height: '100vh' }}>
            <button
                onClick={registerPlayer}
                disabled={!!playerId || isRegistering}
                style={{
                    position: 'absolute',
                    top: '10px',
                    left: '10px',
                    zIndex: 1000,
                    padding: '12px 24px',
                    fontSize: '16px',
                    fontWeight: 'bold',
                    backgroundColor: playerId ? '#4caf50' : '#2196F3',
                    color: 'white',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: playerId ? 'default' : 'pointer',
                    opacity: playerId ? 0.7 : 1,
                    transition: 'all 0.3s ease'
                }}
            >
                {isRegistering ? 'Registering...' : playerId ? `Playing as Player ${playerId}` : 'Register to Play'}
            </button>

            <ReactAsteroids
                onPressedKeysChange={setNewKeys}
                playerId={playerId || 0}
                ships={ships}
                bullets={bullets}
                asteroids={asteroids}
                stars={stars}
                leaderboard={scoreboard}
                pressedKeys={keys}
                gameSize={gameSize} />
        </div>
    )
}

