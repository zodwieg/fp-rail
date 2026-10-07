use std::process::Command;
use std::env;
use std::path::Path;

fn main() {
    // Говорим Cargo перезапускать этот скрипт, только если изменился tauri.conf.json
    println!("cargo:rerun-if-changed=tauri.conf.json");

    // 1. Убиваем старый процесс на Windows перед сборкой, чтобы файл не был заблокирован
    if cfg!(target_os = "windows") {
        let _ = Command::new("taskkill")
            .args(&["/F", "/IM", "FpRail.WebApi*"])
            .output();
    }

    // 2. Определяем пути
    let manifest_dir = env::var("CARGO_MANIFEST_DIR").unwrap();
    let root_dir = Path::new(&manifest_dir).parent().unwrap();
    let backend_dir = root_dir.join("src-backend");
    let target_binaries_dir = Path::new(&manifest_dir).join("binaries");

    // Создаем папку binaries, если её вдруг нет
    std::fs::create_dir_all(&target_binaries_dir).unwrap();

    // 3. Собираем .NET Бэкенд в один файл
    let status = Command::new("dotnet")
        .current_dir(&backend_dir)
        .args(&[
            "publish",
            "-c", "Release",
            "-r", "win-x64",
            "--self-contained", "true",
            "/p:PublishSingleFile=true",
            "/p:PublishReadyToRun=true",
        ])
        .status()
        .expect("Не удалось запустить сборку .NET бэкенда");

    if !status.success() {
        panic!("Ошибка компиляции ASP.NET Core API!");
    }

    // 4. Копируем и правильно переименовываем бинарник с Target Triple
    let target_triple = env::var("TARGET").unwrap(); // Получаем текущий трипл от Cargo
    let source_exe = backend_dir.join("bin").join("Release").join("net9.0").join("win-x64").join("publish").join("FpRail.WebApi.exe");
    let dest_exe = target_binaries_dir.join(format!("FpRail.WebApi-{}.exe", target_triple));

    std::fs::copy(&source_exe, &dest_exe).expect("Не удалось скопировать бинарник в папку Tauri");

    // Также копируем appsettings для работы Kestrel
    let appsettings = backend_dir.join("bin").join("Release").join("net9.0").join("win-x64").join("publish").join("appsettings.json");
    let dest_settings = target_binaries_dir.join("appsettings.json");
    if appsettings.exists() {
        let _ = std::fs::copy(&appsettings, &dest_settings);
    }

    tauri_build::try_build(tauri_build::Attributes::new())
        .expect("failed to run tauri-build");
}
