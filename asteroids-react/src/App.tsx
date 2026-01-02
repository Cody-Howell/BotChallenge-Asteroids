import { useEffect, useState } from 'react'
import './App.css'
import ReactAsteroids from './components/AsteroidsDisplay';

function App() {
  const [gameId, setGameId] = useState(1);
  const [keys, setKeys] = useState<Set<string>>(new Set());
  const [stars, setStars] = useState<{ x: number, y: number }[]>([]);
  const [ships, setShips] = useState<Ship[]>([]);
  const [bullets, setBullets] = useState<InternalGameObject[]>([]);
  const [asteroids, setAsteroids] = useState<Asteroid[]>([]);
  const [gameSize, setGameSize] = useState<{ width: number, height: number }>({ width: 1000, height: 1000 });
  const [scoreboard, setScoreboard] = useState<ScoreType[]>([]);

  const setNewKeys = (keys: Set<string>) => {
    const moveArray = [];
    if (keys.has('w')) moveArray.push("ACCEL 1")
    if (keys.has('s')) moveArray.push("ACCEL -1")
    if (keys.has('d')) moveArray.push("RIGHT")
    if (keys.has('a')) moveArray.push("LEFT")
    if (keys.has(' ')) moveArray.push("FIRE")
    if (keys.has('b')) moveArray.push("BRAKE")

    fetch(`/api/game/move/${gameId}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(moveArray)
    })
    setKeys(keys);
  }

  useEffect(() => {
    // Fetch game size from API
    fetch('/api/game/size')
      .then(res => res.json())
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
      .catch(err => console.error('Failed to fetch game size:', err));
  }, []);

  useEffect(() => {
    if (gameId < 0) return;
    console.log(`Registering to game ${gameId}`)
    const socket = new WebSocket(`ws://localhost:5038/api/game/${gameId}`);

    socket.addEventListener("open", () => {
      console.log("Socket opened.")
    });

    socket.addEventListener("message", (event) => {
      // console.log("Message received:", event.data);
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
    });

    socket.addEventListener("error", (event) => {
      console.error("WebSocket error:", event);
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

  return (
    <>
      {/* <input type='number' value={gameId} onChange={(e) => setGameId(Number.parseInt(e.target.value))} /> */}
      <ReactAsteroids
        onPressedKeysChange={setNewKeys}
        playerId={1}
        ships={ships}
        bullets={bullets}
        asteroids={asteroids}
        stars={stars}
        leaderboard={scoreboard}
        pressedKeys={keys}
        gameSize={gameSize} />
    </>
  )
}

export default App
