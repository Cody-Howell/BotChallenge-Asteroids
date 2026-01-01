type InternalGameObject = {
    x: number;
    y: number;
    orientation: number;
}

type Ship = InternalGameObject & {
    id: number
}