<?php
declare(strict_types=1);

require_once __DIR__ . '/../models/clientes.models.php';

header('Content-Type: application/json; charset=utf-8');

function respond(int $status, array $payload): void {
    http_response_code($status);
    echo json_encode($payload, JSON_UNESCAPED_UNICODE);
    exit;
}

function readJsonBody(): array {
    $raw = file_get_contents('php://input');
    if (!$raw) return [];
    $data = json_decode($raw, true);
    return is_array($data) ? $data : [];
}

function onlyDigits(string $s): string {
    return preg_replace('/\D+/', '', $s) ?? '';
}

function normalizeText(string $s): string {
    $s = trim($s);
    $s = preg_replace('/\s+/', ' ', $s) ?? $s;
    return $s;
}

function countryPrefix(string $paisIso): ?string {
    $map = [
        'EC' => '+593',
        'CO' => '+57',
        'PE' => '+51',
        'VE' => '+58',
        'US' => '+1',
        'ES' => '+34',
    ];
    $paisIso = strtoupper(trim($paisIso));
    return $map[$paisIso] ?? null;
}

function phoneRule(string $paisIso): ?array {
    $rules = [
        'EC' => ['len' => 9,  'groups' => [2,3,4]],  // 98 282 2349
        'CO' => ['len' => 10, 'groups' => [3,3,4]],  // 300 123 4567
        'PE' => ['len' => 9,  'groups' => [3,3,3]],  // 987 654 321
        'VE' => ['len' => 10, 'groups' => [3,3,4]],  // 412 123 4567
        'US' => ['len' => 10, 'groups' => [3,3,4]],  // 202 555 0123
        'ES' => ['len' => 9,  'groups' => [3,3,3]],  // 612 345 678
    ];
    $paisIso = strtoupper(trim($paisIso));
    return $rules[$paisIso] ?? null;
}

function normalizeLocalPhone(string $paisIso, string $raw): string {
    $digits = preg_replace('/\D+/', '', $raw) ?? '';
    $rule = phoneRule($paisIso);

    if ($rule) {
        $len = (int)$rule['len'];

        if (strlen($digits) === $len + 1 && $digits[0] === '0') {
            $digits = substr($digits, 1);
        }

        if (strlen($digits) !== $len) {
            respond(422, ['ok' => false, 'error' => "Teléfono inválido para {$paisIso}. Debe tener {$len} dígitos (sin prefijo)."]);
        }
        return $digits;
    }

    if (strlen($digits) < 6 || strlen($digits) > 15) {
        respond(422, ['ok' => false, 'error' => 'Teléfono inválido (6 a 15 dígitos).']);
    }
    return $digits;
}

function formatLocalPhone(string $paisIso, string $digits): string {
    $rule = phoneRule($paisIso);
    if (!$rule) return $digits;

    $groups = $rule['groups'];
    $pos = 0;
    $parts = [];
    foreach ($groups as $g) {
        $parts[] = substr($digits, $pos, $g);
        $pos += $g;
    }
    return implode(' ', $parts);
}

function buildFormattedPhone(string $paisIso, string $telefonoLocalRaw): string {
    $prefix = countryPrefix($paisIso);
    if ($prefix === null) respond(400, ['ok' => false, 'error' => 'País no soportado']);

    $localDigits = normalizeLocalPhone($paisIso, $telefonoLocalRaw);
    $localFormatted = formatLocalPhone($paisIso, $localDigits);

    return $prefix . ' ' . $localFormatted;
}

function isValidCedulaEC(string $cedulaDigits): bool {
    if (!preg_match('/^\d{10}$/', $cedulaDigits)) return false;

    $prov = (int)substr($cedulaDigits, 0, 2);
    $third = (int)$cedulaDigits[2];
    if ($prov < 1 || $prov > 24) return false;
    if ($third >= 6) return false;

    $coeff = [2,1,2,1,2,1,2,1,2];
    $sum = 0;
    for ($i = 0; $i < 9; $i++) {
        $d = (int)$cedulaDigits[$i];
        $p = $d * $coeff[$i];
        if ($p >= 10) $p -= 9;
        $sum += $p;
    }
    $mod = $sum % 10;
    $check = ($mod === 0) ? 0 : (10 - $mod);
    return $check === (int)$cedulaDigits[9];
}

function isValidIdGeneral(string $idDigits): bool {
    return (bool)preg_match('/^\d{6,20}$/', $idDigits);
}

function validateRequired(array $data, array $fields): void {
    foreach ($fields as $f) {
        if (!isset($data[$f]) || trim((string)$data[$f]) === '') {
            respond(400, ['ok' => false, 'error' => "Campo obligatorio faltante: {$f}"]);
        }
    }
}

$method = $_SERVER['REQUEST_METHOD'] ?? 'GET';
$model = new ClientesModel();

try {
    if ($method === 'GET') {
        $id = isset($_GET['id']) ? (int)$_GET['id'] : 0;

        if ($id > 0) {
            $c = $model->findById($id);
            if (!$c) respond(404, ['ok' => false, 'error' => 'Cliente no encontrado']);
            respond(200, ['ok' => true, 'data' => $c->toArray()]);
        }

        $list = array_map(fn($c) => $c->toArray(), $model->findAll());
        respond(200, ['ok' => true, 'data' => $list]);
    }

    if ($method === 'POST') {
        $data = $_POST ?: readJsonBody();
        validateRequired($data, ['cedula','nombres','apellidos','email','pais_iso','telefono_local']);

        $pais = strtoupper(normalizeText((string)$data['pais_iso']));
        $prefix = countryPrefix($pais);
        if ($prefix === null) respond(400, ['ok' => false, 'error' => 'País no soportado']);

        $cedula = onlyDigits((string)$data['cedula']);
        if ($pais === 'EC') {
            if (!isValidCedulaEC($cedula)) {
                respond(422, ['ok' => false, 'error' => 'Cédula de Ecuador inválida (debe ser 10 dígitos válidos).']);
            }
        } else {
            if (!isValidIdGeneral($cedula)) {
                respond(422, ['ok' => false, 'error' => 'Documento inválido (solo dígitos, 6 a 20).']);
            }
        }

        $nombres = normalizeText((string)$data['nombres']);
        $apellidos = normalizeText((string)$data['apellidos']);
        $email = strtolower(trim((string)$data['email']));
        if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
            respond(422, ['ok' => false, 'error' => 'Email inválido.']);
        }

        $telefono = buildFormattedPhone($pais, (string)$data['telefono_local']);

        if ($model->existsCedula($cedula)) respond(409, ['ok' => false, 'error' => 'La cédula/documento ya existe.']);
        if ($model->existsEmail($email)) respond(409, ['ok' => false, 'error' => 'El email ya existe.']);
        if ($model->existsTelefono($telefono)) respond(409, ['ok' => false, 'error' => 'El teléfono ya existe.']);

        $cliente = new Cliente(null, $cedula, $nombres, $apellidos, $email, $pais, $telefono);
        $newId = $model->create($cliente);
        $created = $model->findById($newId);

        respond(201, ['ok' => true, 'data' => $created ? $created->toArray() : null]);
    }

    if ($method === 'PUT') {
        $id = isset($_GET['id']) ? (int)$_GET['id'] : 0;
        if ($id <= 0) respond(400, ['ok' => false, 'error' => 'ID requerido']);

        $data = readJsonBody();
        validateRequired($data, ['cedula','nombres','apellidos','email','pais_iso','telefono_local']);

        if (!$model->findById($id)) respond(404, ['ok' => false, 'error' => 'Cliente no encontrado']);

        $pais = strtoupper(normalizeText((string)$data['pais_iso']));
        $prefix = countryPrefix($pais);
        if ($prefix === null) respond(400, ['ok' => false, 'error' => 'País no soportado']);

        $cedula = onlyDigits((string)$data['cedula']);
        if ($pais === 'EC') {
            if (!isValidCedulaEC($cedula)) respond(422, ['ok' => false, 'error' => 'Cédula de Ecuador inválida.']);
        } else {
            if (!isValidIdGeneral($cedula)) respond(422, ['ok' => false, 'error' => 'Documento inválido.']);
        }

        $nombres = normalizeText((string)$data['nombres']);
        $apellidos = normalizeText((string)$data['apellidos']);
        $email = strtolower(trim((string)$data['email']));
        if (!filter_var($email, FILTER_VALIDATE_EMAIL)) respond(422, ['ok' => false, 'error' => 'Email inválido.']);

        $telefono = buildFormattedPhone($pais, (string)$data['telefono_local']);

        if ($model->existsCedula($cedula, $id)) respond(409, ['ok' => false, 'error' => 'La cédula/documento ya existe.']);
        if ($model->existsEmail($email, $id)) respond(409, ['ok' => false, 'error' => 'El email ya existe.']);
        if ($model->existsTelefono($telefono, $id)) respond(409, ['ok' => false, 'error' => 'El teléfono ya existe.']);

        $cliente = new Cliente($id, $cedula, $nombres, $apellidos, $email, $pais, $telefono);
        $model->update($id, $cliente);

        $updated = $model->findById($id);
        respond(200, ['ok' => true, 'data' => $updated ? $updated->toArray() : null]);
    }

    if ($method === 'DELETE') {
        $id = isset($_GET['id']) ? (int)$_GET['id'] : 0;
        if ($id <= 0) respond(400, ['ok' => false, 'error' => 'ID requerido']);

        if (!$model->findById($id)) respond(404, ['ok' => false, 'error' => 'Cliente no encontrado']);

        $model->delete($id);
        respond(200, ['ok' => true, 'data' => ['deleted' => true]]);
    }

    respond(405, ['ok' => false, 'error' => 'Método no permitido']);
} catch (PDOException $e) {
    if (($e->getCode() ?? '') === '23000') {
        respond(409, ['ok' => false, 'error' => 'Datos duplicados (restricción UNIQUE).']);
    }
    respond(500, ['ok' => false, 'error' => 'Error de base de datos']);
} catch (Throwable $e) {
    respond(500, ['ok' => false, 'error' => 'Error interno del servidor']);
}
