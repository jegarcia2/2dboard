# Prototipo CAD en C# con Canvas

Este proyecto es un **prototipo de aplicación CAD** desarrollado desde cero en **C#** utilizando **Canvas**. El objetivo es crear una plataforma sencilla para la **creación y edición de dibujos 2D** con funcionalidades personalizadas, como la **selección de idioma**, **borrado dinámico**, **visualización por coordenadas** y mucho más.

Este prototipo pone énfasis en la organización eficiente y la **interactividad** del usuario, con una interfaz fluida y herramientas intuitivas.

## Características

### **Selección de Lenguaje**
- El proyecto incluye **soporte multilingüe** con opciones para mostrar la interfaz en **español e inglés**, utilizando archivos `.resx` para gestionar los textos. Esto garantiza que la aplicación sea accesible a una audiencia más amplia y pueda ser fácilmente extendida a otros idiomas en el futuro.

### **Herramientas de Dibujo: Líneas y Círculos**
- Permite al usuario crear **líneas** y **círculos** con una interfaz directa y funcional. Las herramientas permiten modificar las propiedades de los elementos de manera dinámica, mejorando la experiencia del usuario al no tener que realizar pasos adicionales para editar sus dibujos.
  
  - **Dibujo de Líneas:** Las líneas se pueden crear de forma rápida con distintas configuraciones de color.
  ![LINE  DRAWING](Resources\captures\draw.gif)
  - **Dibujo de Círculos:** Similar al dibujo de líneas, pero con opciones para ajustar el radio y color de los círculos.
  - **Selección de Color:** Cada figura creada permite cambiar su color a través de un menú de opciones dinámico.

### **Propiedades Dinámicas**
- Los **atributos de las líneas** y **círculos** pueden ser modificados en **tiempo real**. Solo basta con seleccionar el objeto y sus propiedades se actualizan inmediatamente (como el color, el grosor de línea, etc.), lo cual facilita el diseño de planos sin necesidad de realizar modificaciones complicadas.

### **Borrado Dinámico**
- El sistema permite eliminar objetos de forma sencilla, con un estilo de **borrado recortado**: la herramienta de borrado elimina solo los objetos seleccionados al pasar por encima de ellos, lo que facilita la edición en tiempo real sin complicaciones.

### **Visualización por Coordenadas**
- Se muestra una **guía de coordenadas** en la pantalla para facilitar la creación y edición de objetos. Esta funcionalidad permite a los usuarios crear dibujos con precisión, garantizando que las líneas y los círculos estén alineados y sean simétricos.

### **Funciones Beta**
- Se incluyen algunas funcionalidades experimentales que están siendo desarrolladas o mejoradas:
  - **Snap to Point:** Ajuste automático al punto más cercano al mover el mouse.
  - **Grid (Rejilla):** Habilitación de una cuadrícula visual para facilitar el dibujo.
  - **Regresar al Centro:** Opción para centrar el lienzo en la pantalla.

### **Futuras Mejoras**
- **Transición de 2D a 3D:** Una de las futuras características que planeo agregar es la capacidad de visualizar los objetos en 3D (al menos de manera no editable), comenzando por los círculos.
- **Acotación:** Se quiere permitir que los usuarios obtengan medidas de los objetos dibujados, una función fundamental para los programas CAD completos.
- **Mejoras en la precisión y control del Snap to Grid**: Actualmente en fase beta, esta herramienta se ampliará para ser más precisa y útil en planos complejos.

## Tecnologías
Este proyecto utiliza una serie de tecnologías clave para su desarrollo:

- **C#**: Lenguaje principal utilizado para el desarrollo de la aplicación.
- **Canvas**: Permite la creación y renderizado de los elementos gráficos (líneas, círculos) en la interfaz.
- **Resx**: Para la internacionalización y la creación de interfaces multilingües.
- **WinForms**: Para la interfaz de usuario, utilizando controles estándar de Windows.
  
## Instalación

Para comenzar a usar este proyecto en tu entorno local, sigue los siguientes pasos:

1. **Clona este repositorio:**
   ```bash
   git clone https://github.com/jegarcia2/2dboard.git
