<?php
declare(strict_types=1);

require_once __DIR__ . '/../config/conexion.php';
require_once __DIR__ . '/cliente.php';

final class ClientesModel
{
    private PDO $db;

    public function __construct()
    {
        $this->db = Conexion::get();
    }

    public function create(Cliente $c): int
    {
        $sql = "INSERT INTO clientes (cedula, nombres, apellidos, email, pais_iso, telefono)
                VALUES (:cedula, :nombres, :apellidos, :email, :pais_iso, :telefono)";
        $stmt = $this->db->prepare($sql);
        $stmt->execute([
            ':cedula' => $c->getCedula(),
            ':nombres' => $c->getNombres(),
            ':apellidos' => $c->getApellidos(),
            ':email' => $c->getEmail(),
            ':pais_iso' => $c->getPaisIso(),
            ':telefono' => $c->getTelefono(),
        ]);
        return (int)$this->db->lastInsertId();
    }

    public function findAll(): array
    {
        $rows = $this->db->query("SELECT id, cedula, nombres, apellidos, email, pais_iso, telefono
                                 FROM clientes ORDER BY id DESC")->fetchAll();
        return array_map(fn($r) => Cliente::fromArray($r), $rows);
    }

    public function findById(int $id): ?Cliente
    {
        $stmt = $this->db->prepare("SELECT id, cedula, nombres, apellidos, email, pais_iso, telefono
                                    FROM clientes WHERE id = :id LIMIT 1");
        $stmt->execute([':id' => $id]);
        $row = $stmt->fetch();
        return $row ? Cliente::fromArray($row) : null;
    }

    public function update(int $id, Cliente $c): bool
    {
        $sql = "UPDATE clientes
                SET cedula = :cedula,
                    nombres = :nombres,
                    apellidos = :apellidos,
                    email = :email,
                    pais_iso = :pais_iso,
                    telefono = :telefono
                WHERE id = :id";
        $stmt = $this->db->prepare($sql);
        return $stmt->execute([
            ':cedula' => $c->getCedula(),
            ':nombres' => $c->getNombres(),
            ':apellidos' => $c->getApellidos(),
            ':email' => $c->getEmail(),
            ':pais_iso' => $c->getPaisIso(),
            ':telefono' => $c->getTelefono(),
            ':id' => $id
        ]);
    }

    public function delete(int $id): bool
    {
        $stmt = $this->db->prepare("DELETE FROM clientes WHERE id = :id");
        return $stmt->execute([':id' => $id]);
    }

    public function existsCedula(string $cedula, ?int $excludeId = null): bool
    {
        $sql = "SELECT 1 FROM clientes WHERE cedula = :cedula";
        $params = [':cedula' => $cedula];

        if ($excludeId !== null) {
            $sql .= " AND id <> :id";
            $params[':id'] = $excludeId;
        }

        $sql .= " LIMIT 1";
        $stmt = $this->db->prepare($sql);
        $stmt->execute($params);
        return (bool)$stmt->fetchColumn();
    }

    public function existsEmail(string $email, ?int $excludeId = null): bool
    {
        $sql = "SELECT 1 FROM clientes WHERE email = :email";
        $params = [':email' => $email];

        if ($excludeId !== null) {
            $sql .= " AND id <> :id";
            $params[':id'] = $excludeId;
        }

        $sql .= " LIMIT 1";
        $stmt = $this->db->prepare($sql);
        $stmt->execute($params);
        return (bool)$stmt->fetchColumn();
    }

    public function existsTelefono(string $telefono, ?int $excludeId = null): bool
    {
        $sql = "SELECT 1 FROM clientes WHERE telefono = :telefono";
        $params = [':telefono' => $telefono];

        if ($excludeId !== null) {
            $sql .= " AND id <> :id";
            $params[':id'] = $excludeId;
        }

        $sql .= " LIMIT 1";
        $stmt = $this->db->prepare($sql);
        $stmt->execute($params);
        return (bool)$stmt->fetchColumn();
    }
}
