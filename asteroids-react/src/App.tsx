import { useEffect, useState } from 'react'
import './App.css'
import ReactAsteroids from './components/AsteroidsDisplay';

function App() {
  const [gameId, setGameId] = useState(1);
  const [keys, setKeys] = useState<Set<string>>(new Set());
  const [stars, setStars] = useState<{x: number, y: number}[]>([]);
  const [ships, setShips] = useState<Ship[]>([]);

  const setNewKeys = (keys: Set<string>) => {
    const moveArray = [];
    if (keys.has('w')) moveArray.push("ACCEL 1")
    if (keys.has('s')) moveArray.push("ACCEL -1")
    if (keys.has('d')) moveArray.push("RIGHT")
    if (keys.has('a')) moveArray.push("LEFT")
    if (keys.has(' ')) moveArray.push("FIRE")
    if (keys.has('b')) moveArray.push("BRAKE")

    console.log("Sending update: ", moveArray);

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
    const maxX = window.innerWidth;
    const maxY = window.innerHeight;

    const stars = [];
    const numOfStars = 20;
    for (let i = 0; i < numOfStars; i++) {
      stars.push({x: Math.random() * maxX, y: Math.random() * maxY})
    }
    setStars(stars);
  }, []);

  useEffect(() => {
    if (gameId < 0) return;
    console.log(`Registering to game ${gameId}`)
    const socket = new WebSocket(`ws://localhost:5038/api/game/${gameId}`);

    socket.addEventListener("open", () => {
      console.log("Socket opened.")
    });

    socket.addEventListener("message", (event) => {
      console.log("Message received:", event.data);
      const json = JSON.parse(event.data) as {Players: string[], Asteroids: string[], Bullets: string[]};
      const players: Ship[] = [];
      for (var player of json.Players) {
        const items = player.split(' ');
        players.push({
          x: Number.parseFloat(items[1]),
          y: Number.parseFloat(items[2]),
          orientation: (Number.parseFloat(items[3]) + 90) % 360,
          id: Number.parseInt(items[0])
        })
      }
      setShips(players);
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
        playerId={0}
        ships={ships}
        bullets={[]}
        asteroids={[]}
        stars={stars}
        leaderboard={[]} 
        pressedKeys={keys} />
    </>
  )
}

export default App
