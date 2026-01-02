type InternalGameObject = {
    x: number;
    y: number;
    orientation: number;
}

type Ship = InternalGameObject & {
    id: number; 
}

type Asteroid = InternalGameObject & {
    radius: number;
}

type ScoreType = { id: number, score: number, health: number }