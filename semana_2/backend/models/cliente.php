<?php
declare(strict_types=1);

final class Cliente
{
    private ?int $id;
    private string $cedula;
    private string $nombres;
    private string $apellidos;
    private string $email;
    private string $paisIso;   // EC, CO, PE...
    private string $telefono;  // E.164: +593987654321

    public function __construct(
        ?int $id,
        string $cedula,
        string $nombres,
        string $apellidos,
        string $email,
        string $paisIso,
        string $telefono
    ) {
        $this->id = $id;
        $this->cedula = $cedula;
        $this->nombres = $nombres;
        $this->apellidos = $apellidos;
        $this->email = $email;
        $this->paisIso = $paisIso;
        $this->telefono = $telefono;
    }

    public function getId(): ?int { return $this->id; }
    public function getCedula(): string { return $this->cedula; }
    public function getNombres(): string { return $this->nombres; }
    public function getApellidos(): string { return $this->apellidos; }
    public function getEmail(): string { return $this->email; }
    public function getPaisIso(): string { return $this->paisIso; }
    public function getTelefono(): string { return $this->telefono; }

    public function setCedula(string $v): void { $this->cedula = $v; }
    public function setNombres(string $v): void { $this->nombres = $v; }
    public function setApellidos(string $v): void { $this->apellidos = $v; }
    public function setEmail(string $v): void { $this->email = $v; }
    public function setPaisIso(string $v): void { $this->paisIso = $v; }
    public function setTelefono(string $v): void { $this->telefono = $v; }

    public static function fromArray(array $row): self
    {
        return new self(
            isset($row['id']) ? (int)$row['id'] : null,
            (string)$row['cedula'],
            (string)$row['nombres'],
            (string)$row['apellidos'],
            (string)$row['email'],
            (string)$row['pais_iso'],
            (string)$row['telefono']
        );
    }

    public function toArray(): array
    {
        return [
            'id' => $this->id,
            'cedula' => $this->cedula,
            'nombres' => $this->nombres,
            'apellidos' => $this->apellidos,
            'email' => $this->email,
            'pais_iso' => $this->paisIso,
            'telefono' => $this->telefono,
        ];
    }
}
