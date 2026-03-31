import * as THREE from 'https://unpkg.com/three@0.161.0/build/three.module.js';
import { PointerLockControls } from 'https://unpkg.com/three@0.161.0/examples/jsm/controls/PointerLockControls.js';

const WORLD_X = 24;
const WORLD_Y = 16;
const WORLD_Z = 24;
const WATER_LEVEL = 5;

const BLOCK = {
  AIR: 0,
  DIRT: 1,
  GLASS: 2,
  WATER: 3,
};

const BLOCK_LABEL = {
  [BLOCK.DIRT]: 'Земля',
  [BLOCK.GLASS]: 'Стекло',
  [BLOCK.WATER]: 'Вода',
};

const materials = {
  [BLOCK.DIRT]: new THREE.MeshLambertMaterial({ color: 0x8b5a2b }),
  [BLOCK.GLASS]: new THREE.MeshLambertMaterial({
    color: 0x99ddff,
    transparent: true,
    opacity: 0.35,
  }),
  [BLOCK.WATER]: new THREE.MeshLambertMaterial({
    color: 0x2f6aff,
    transparent: true,
    opacity: 0.5,
  }),
};

const scene = new THREE.Scene();
scene.background = new THREE.Color(0x87ceeb);

const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

const ambient = new THREE.AmbientLight(0xffffff, 0.45);
scene.add(ambient);

const sun = new THREE.DirectionalLight(0xffffff, 0.85);
sun.position.set(25, 40, 20);
scene.add(sun);

const controls = new PointerLockControls(camera, document.body);
scene.add(controls.getObject());

const hint = document.getElementById('hint');
document.body.addEventListener('click', () => controls.lock());
controls.addEventListener('lock', () => { hint.textContent = ''; });
controls.addEventListener('unlock', () => { hint.textContent = 'Кликни по экрану для захвата мыши'; });

const selectedNode = document.getElementById('selected');
let selectedBlock = BLOCK.DIRT;

const world = Array.from({ length: WORLD_X }, () =>
  Array.from({ length: WORLD_Y }, () => Array.from({ length: WORLD_Z }, () => BLOCK.AIR)),
);

const meshMap = new Map();
const cubeGeometry = new THREE.BoxGeometry(1, 1, 1);
const raycaster = new THREE.Raycaster();

function keyFor(x, y, z) {
  return `${x},${y},${z}`;
}

function inBounds(x, y, z) {
  return x >= 0 && y >= 0 && z >= 0 && x < WORLD_X && y < WORLD_Y && z < WORLD_Z;
}

function setBlock(x, y, z, type) {
  if (!inBounds(x, y, z)) return;
  const prev = world[x][y][z];
  if (prev === type) return;

  world[x][y][z] = type;
  const k = keyFor(x, y, z);
  const existing = meshMap.get(k);
  if (existing) {
    scene.remove(existing);
    meshMap.delete(k);
  }

  if (type !== BLOCK.AIR) {
    const mesh = new THREE.Mesh(cubeGeometry, materials[type]);
    mesh.position.set(x + 0.5, y + 0.5, z + 0.5);
    mesh.userData = { x, y, z, type };
    scene.add(mesh);
    meshMap.set(k, mesh);
  }
}

function generateWorld() {
  for (let x = 0; x < WORLD_X; x++) {
    for (let z = 0; z < WORLD_Z; z++) {
      const height = 3 + Math.floor(Math.sin(x * 0.45) + Math.cos(z * 0.45));
      for (let y = 0; y <= height; y++) {
        setBlock(x, y, z, BLOCK.DIRT);
      }
      if (x % 9 === 0 && z % 9 === 0) {
        setBlock(x, height + 1, z, BLOCK.GLASS);
      }
    }
  }

  for (let x = 6; x < 18; x++) {
    for (let z = 6; z < 18; z++) {
      setBlock(x, WATER_LEVEL, z, BLOCK.WATER);
    }
  }
}

const velocity = new THREE.Vector3();
const moveDir = { forward: false, back: false, left: false, right: false, down: false };
let canJump = false;

controls.getObject().position.set(10, 10, 10);

const playerHeight = 1.7;
const gravity = 20;
const moveSpeed = 8;
const jumpSpeed = 9;

function playerCell(pos) {
  return {
    x: Math.floor(pos.x),
    yFeet: Math.floor(pos.y - playerHeight),
    yHead: Math.floor(pos.y - 0.2),
    z: Math.floor(pos.z),
  };
}

function isSolid(type) {
  return type === BLOCK.DIRT || type === BLOCK.GLASS;
}

function collidesAt(position) {
  const c = playerCell(position);
  for (let y = c.yFeet; y <= c.yHead; y++) {
    if (!inBounds(c.x, y, c.z)) return true;
    if (isSolid(world[c.x][y][c.z])) return true;
  }
  return false;
}

function applyMovement(delta) {
  const obj = controls.getObject();

  velocity.x -= velocity.x * 8 * delta;
  velocity.z -= velocity.z * 8 * delta;
  velocity.y -= gravity * delta;

  const dir = new THREE.Vector3();
  dir.z = Number(moveDir.forward) - Number(moveDir.back);
  dir.x = Number(moveDir.right) - Number(moveDir.left);
  dir.normalize();

  if (moveDir.forward || moveDir.back) velocity.z -= dir.z * moveSpeed * delta;
  if (moveDir.left || moveDir.right) velocity.x -= dir.x * moveSpeed * delta;
  if (moveDir.down) velocity.y -= 10 * delta;

  const old = obj.position.clone();

  controls.moveRight(-velocity.x * delta);
  controls.moveForward(-velocity.z * delta);

  const afterXZ = obj.position.clone();
  if (collidesAt(afterXZ)) {
    obj.position.x = old.x;
    obj.position.z = old.z;
    velocity.x = 0;
    velocity.z = 0;
  }

  obj.position.y += velocity.y * delta;

  if (collidesAt(obj.position)) {
    if (velocity.y < 0) canJump = true;
    velocity.y = 0;
    obj.position.y = old.y;
  }

  if (obj.position.y < 2) {
    obj.position.y = 8;
    velocity.set(0, 0, 0);
  }
}

function updateSelectedText() {
  selectedNode.textContent = `Выбран блок: ${BLOCK_LABEL[selectedBlock]}`;
}

function handleSelect(e) {
  if (e.code === 'Digit1') selectedBlock = BLOCK.DIRT;
  if (e.code === 'Digit2') selectedBlock = BLOCK.GLASS;
  if (e.code === 'Digit3') selectedBlock = BLOCK.WATER;
  updateSelectedText();
}

function placeOrBreak(place) {
  if (!controls.isLocked) return;

  raycaster.setFromCamera(new THREE.Vector2(0, 0), camera);
  const hits = raycaster.intersectObjects([...meshMap.values()]);
  if (!hits.length) return;

  const hit = hits[0];
  const { x, y, z } = hit.object.userData;

  if (!place) {
    setBlock(x, y, z, BLOCK.AIR);
    return;
  }

  const normal = hit.face.normal;
  const nx = x + normal.x;
  const ny = y + normal.y;
  const nz = z + normal.z;

  const px = Math.floor(controls.getObject().position.x);
  const py = Math.floor(controls.getObject().position.y - 1);
  const pz = Math.floor(controls.getObject().position.z);
  if (nx === px && ny === py && nz === pz) return;

  setBlock(nx, ny, nz, selectedBlock);
}

document.addEventListener('contextmenu', (e) => e.preventDefault());
document.addEventListener('mousedown', (e) => {
  if (e.button === 0) placeOrBreak(false);
  if (e.button === 2) placeOrBreak(true);
});

document.addEventListener('keydown', (e) => {
  if (e.code === 'KeyW') moveDir.forward = true;
  if (e.code === 'KeyS') moveDir.back = true;
  if (e.code === 'KeyA') moveDir.left = true;
  if (e.code === 'KeyD') moveDir.right = true;
  if (e.code === 'ShiftLeft') moveDir.down = true;

  if (e.code === 'Space' && canJump) {
    velocity.y += jumpSpeed;
    canJump = false;
  }

  handleSelect(e);
});

document.addEventListener('keyup', (e) => {
  if (e.code === 'KeyW') moveDir.forward = false;
  if (e.code === 'KeyS') moveDir.back = false;
  if (e.code === 'KeyA') moveDir.left = false;
  if (e.code === 'KeyD') moveDir.right = false;
  if (e.code === 'ShiftLeft') moveDir.down = false;
});

window.addEventListener('resize', () => {
  camera.aspect = window.innerWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(window.innerWidth, window.innerHeight);
});

let waterTimer = 0;
function updateWater(delta) {
  waterTimer += delta;
  if (waterTimer < 0.18) return;
  waterTimer = 0;

  const flows = [];

  for (let x = 0; x < WORLD_X; x++) {
    for (let y = 0; y < WORLD_Y; y++) {
      for (let z = 0; z < WORLD_Z; z++) {
        if (world[x][y][z] !== BLOCK.WATER) continue;

        const below = { x, y: y - 1, z };
        if (inBounds(below.x, below.y, below.z) && world[below.x][below.y][below.z] === BLOCK.AIR) {
          flows.push({ from: { x, y, z }, to: below, strong: true });
          continue;
        }

        const dirs = [
          { x: 1, y, z },
          { x: -1, y, z },
          { x, y, z: 1 },
          { x, y, z: -1 },
        ];

        for (const side of dirs) {
          if (inBounds(side.x, side.y, side.z) && world[side.x][side.y][side.z] === BLOCK.AIR) {
            flows.push({ from: { x, y, z }, to: side, strong: false });
          }
        }
      }
    }
  }

  for (const flow of flows) {
    if (world[flow.to.x][flow.to.y][flow.to.z] !== BLOCK.AIR) continue;
    setBlock(flow.to.x, flow.to.y, flow.to.z, BLOCK.WATER);
    if (flow.strong) setBlock(flow.from.x, flow.from.y, flow.from.z, BLOCK.AIR);
  }
}

const clock = new THREE.Clock();
function animate() {
  requestAnimationFrame(animate);
  const delta = Math.min(0.05, clock.getDelta());

  applyMovement(delta);
  updateWater(delta);

  renderer.render(scene, camera);
}

generateWorld();
updateSelectedText();
animate();
