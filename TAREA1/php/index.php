<?php
declare(strict_types=1);
session_start();
header('Content-Type: text/html; charset=utf-8');

/* ===== CONFIG BD (XAMPP) ===== */
$dbHost = "127.0.0.1";
$dbName = "tarea1";
$dbUser = "root";
$dbPass = "";
$dsn = "mysql:host={$dbHost};dbname={$dbName};charset=utf8mb4";

/* ===== CSRF ===== */
if (empty($_SESSION["csrf"])) {
  $_SESSION["csrf"] = bin2hex(random_bytes(32));
}
$csrf = $_SESSION["csrf"];

/* ===== Helpers ===== */
function e(string $s): string {
  return htmlspecialchars($s, ENT_QUOTES, "UTF-8");
}

function normalizeNumber(string $v): string {
  // Permite que el usuario escriba 10,5 y lo convierte a 10.5
  return str_replace(",", ".", trim($v));
}

function isValidNumber(string $v): bool {
  return preg_match('/^-?\d+(\.\d+)?$/', $v) === 1;
}

function fmtDecimal($v): string {
  // MariaDB DECIMAL suele venir como string: "10.0000"
  $s = (string)$v;
  // Si viene con coma, normaliza
  $s = str_replace(",", ".", $s);
  // Quita ceros sobrantes: 10.0000 -> 10 ; 10.5000 -> 10.5
  $s = rtrim(rtrim($s, "0"), ".");
  // Si queda vacío (caso "0.0000"), devuelve 0
  return $s === "" ? "0" : $s;
}

/* ===== Estado ===== */
$errors = [];
$result = null;

$op1 = "";
$op2 = "";
$operator = "";

/* ===== Conexión ===== */
try {
  $pdo = new PDO($dsn, $dbUser, $dbPass, [
    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
  ]);
} catch (Throwable $e) {
  $pdo = null;
  $errors[] = "No se pudo conectar a la base de datos.";
}

/* ===== POST ===== */
if ($_SERVER["REQUEST_METHOD"] === "POST") {
  $posted = (string)($_POST["csrf"] ?? "");
  if (!hash_equals($csrf, $posted)) {
    $errors[] = "CSRF inválido. Recarga e intenta de nuevo.";
  }

  $op1 = normalizeNumber((string)($_POST["op1"] ?? ""));
  $op2 = normalizeNumber((string)($_POST["op2"] ?? ""));
  $operator = trim((string)($_POST["operator"] ?? ""));

  if ($op1 === "" || $op2 === "" || $operator === "") {
    $errors[] = "Todos los campos son obligatorios.";
  }

  if ($op1 !== "" && !isValidNumber($op1)) $errors[] = "Número 1 inválido.";
  if ($op2 !== "" && !isValidNumber($op2)) $errors[] = "Número 2 inválido.";

  $allowed = ["+", "-", "*", "/"];
  if ($operator !== "" && !in_array($operator, $allowed, true)) {
    $errors[] = "Operación no permitida.";
  }

  if (!$errors) {
    $a = (float)$op1;
    $b = (float)$op2;

    switch ($operator) {
      case "+": $result = $a + $b; break;
      case "-": $result = $a - $b; break;
      case "*": $result = $a * $b; break;
      case "/":
        if ($b == 0.0) $errors[] = "No se puede dividir para cero.";
        else $result = $a / $b;
        break;
    }

    if (!$errors && $pdo !== null) {
      // Guardamos con 4 decimales en BD, pero al mostrar formateamos bonito
      $stmt = $pdo->prepare(
        "INSERT INTO calculations (operand1, operand2, operator, result)
         VALUES (:a, :b, :op, :r)"
      );
      $stmt->execute([
        ":a" => $a,
        ":b" => $b,
        ":op" => $operator,
        ":r" => $result,
      ]);
    }
  }
}

/* ===== Últimos cálculos ===== */
$latest = [];
if ($pdo !== null) {
  $latest = $pdo->query(
    "SELECT operand1, operator, operand2, result, created_at
     FROM calculations
     ORDER BY created_at DESC
     LIMIT 10"
  )->fetchAll();
}
?>
<!doctype html>
<html lang="es">
<head>
  <meta charset="utf-8"/>
  <meta name="viewport" content="width=device-width, initial-scale=1"/>
  <title>Calculadora PHP</title>
  <link rel="stylesheet" href="styles.css"/>
</head>
<body>
  <main class="card">
    <h1>Calculadora (PHP)</h1>


    <?php if ($errors): ?>
      <div class="alert">
        <ul>
          <?php foreach ($errors as $er): ?>
            <li><?= e($er) ?></li>
          <?php endforeach; ?>
        </ul>
      </div>
    <?php endif; ?>

    <form method="post" class="form" novalidate>
      <input type="hidden" name="csrf" value="<?= e($csrf) ?>"/>

      <div class="field">
        <label for="op1">Número 1</label>
        <input id="op1" name="op1" value="<?= e($op1) ?>" inputmode="decimal" placeholder="Ej: 10 o 10.5" required/>
      </div>

      <div class="field">
        <label for="operator">Operación</label>
        <select id="operator" name="operator" required>
          <option value="">Seleccione…</option>
          <?php foreach (["+", "-", "*", "/"] as $op): ?>
            <option value="<?= e($op) ?>" <?= $operator === $op ? "selected" : "" ?>><?= e($op) ?></option>
          <?php endforeach; ?>
        </select>
      </div>

      <div class="field">
        <label for="op2">Número 2</label>
        <input id="op2" name="op2" value="<?= e($op2) ?>" inputmode="decimal" placeholder="Ej: 10 o 10.5" required/>
      </div>

      <button class="btn" type="submit">Calcular</button>
    </form>

    <?php if ($result !== null && !$errors): ?>
      <div class="ok">Resultado: <strong><?= e(fmtDecimal($result)) ?></strong></div>
    <?php endif; ?>

    <h2>Últimos 10 cálculos</h2>

    <?php if (!$latest): ?>
      <p class="muted">Aún no hay registros.</p>
    <?php else: ?>
      <div class="tableWrap">
        <table>
          <thead>
            <tr>
              <th>Operación</th>
              <th>Resultado</th>
              <th>Fecha</th>
            </tr>
          </thead>
          <tbody>
            <?php foreach ($latest as $r): ?>
              <tr>
                <td><?= e(fmtDecimal($r["operand1"])." ".$r["operator"]." ".fmtDecimal($r["operand2"])) ?></td>
                <td><?= e(fmtDecimal($r["result"])) ?></td>
                <td><?= e((string)$r["created_at"]) ?></td>
              </tr>
            <?php endforeach; ?>
          </tbody>
        </table>
      </div>
    <?php endif; ?>
  </main>
</body>
</html>
