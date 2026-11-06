# CEVI · Proyecto Clínica

Aplicación Unity para Android que simula en primera persona una visita médica (recepción, sala de espera y consulta). Está pensada para la exposición gradual a entornos clínicos de personas con TEA, y registra métricas de cada sesión.

## Requisitos

- Unity **6000.3.7f1** (ver `ProjectSettings/ProjectVersion.txt`)
- Universal Render Pipeline 17.3 e Input System 1.18 (ya incluidos en `Packages/manifest.json`)
- Módulo de Android Build Support para generar el APK

## Cómo ejecutarlo

1. Abrir la carpeta del proyecto desde Unity Hub.
2. Abrir `Assets/Scenes/Bootstrap.unity` y pulsar Play. Esta escena crea los gestores globales y carga el menú.
3. En el Editor, arrastrar con el ratón sustituye al dedo para mover la cámara.

Para compilar: `File > Build Profiles`, cambiar la plataforma a Android y mantener las escenas en este orden: `Bootstrap`, `MainMenu`, `Reception`, `WaitingRoom`, `Consultation`.

## Flujo de la aplicación

`Bootstrap` → `MainMenu` (pseudónimo, ajustes, selección de escena, jugar) → `Reception` → `WaitingRoom` → `Consultation` → `MainMenu`

## Funcionalidades

- Vista en primera persona con control táctil o por giroscopio (se cambia desde la propia escena).
- Detección de agitación del dispositivo, registrada en las métricas.
- Tiempo de espera configurable (2, 5 o 10 minutos) con temporizador por escena.
- Botones de estado emocional, registrados como eventos.
- Consulta guiada por fases: voz del doctor, animación y texto sincronizados.
- Ajustes de volumen de ambiente y efectos, brillo y tamaño del texto de los diálogos.
- Exportación de las métricas con el menú de compartir del sistema (NativeShare).

## Estructura

| Ruta | Contenido |
|---|---|
| `Assets/Scenes` | Las 5 escenas de la aplicación |
| `Assets/Scripts/SystemArchitecture` | Gestores globales: ajustes, audio, brillo, métricas, arranque y menú |
| `Assets/Scripts/InGameScripts` | Interfaz dentro de las escenas: botones, temporizador, cambio de escena |
| `Assets/Scripts/Dialogs` | Diálogos sincronizados con las fases del doctor |
| `Assets/Scripts/Camera` | Control de cámara táctil y por giroscopio |
| `Assets/Scripts/DeslumbramientoLinterna` | Efecto de deslumbramiento de la linterna |
| `Assets/Scripts/Volume` | Controles de volumen en la interfaz |
| `Assets/3DModels` | Modelos, animaciones del doctor y personajes extra |
| `Assets/Audios` | Música ambiente, sonidos de interfaz, voces por fase y mezcladores |
| `Assets/Plugins/NativeShare` | Plugin para compartir archivos en Android e iOS |

## Datos que guarda

Todo se guarda en `Application.persistentDataPath`:

- `settings.json`: ajustes de la aplicación.
- `metrics/<pseudónimo>_<fecha>.json`: usuario, inicio de sesión, eventos con su hora (estado emocional, agitación) y un resumen de cada partida.

El usuario se identifica solo con un pseudónimo, sin datos personales.

## Pendiente

- El identificador de paquete de Android sigue siendo el de la plantilla de Unity y el nombre del producto es `MiPrimeraRevision`.
- No hay firma (keystore) configurada para publicar.
- La carpeta `Assets/TutorialInfo` es de la plantilla y puede eliminarse.
