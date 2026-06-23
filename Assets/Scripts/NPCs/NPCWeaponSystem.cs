using System.Collections;
using UnityEngine;

public class NPCWeaponSystem : WeaponSystem {

    public bool TimedPrimaryFire(float time) {
        if (PrimaryInputStatus != InputStatus.Pressed && PrimaryInputStatus != InputStatus.Held) {
            StartCoroutine(TimedPrimaryFireCoroutine(time));
            return true;
        }
        return false;
    }

    public bool TimedSecondaryFire(float time) {
        if (PrimaryInputStatus != InputStatus.Pressed && PrimaryInputStatus != InputStatus.Held) {
            StartCoroutine(TimedSecondaryFireCoroutine(time));
            return true;
        }
        return false;
    }

    public bool StartPrimaryFire() {
        if (PrimaryInputStatus != InputStatus.Pressed && PrimaryInputStatus != InputStatus.Held) {
            StartCoroutine(StartPrimaryFireCoroutine());
            return true;
        }
        return false;
    }

    public bool EndPrimaryFire() {
        if (PrimaryInputStatus != InputStatus.Released && PrimaryInputStatus != InputStatus.NoInput) {
            StartCoroutine(EndPrimaryFireCoroutine());
            return true;
        }
        return false;
    }

    public bool StartSecondaryFire() {
        if (SecondaryInputStatus != InputStatus.Pressed && SecondaryInputStatus != InputStatus.Held) {
            StartCoroutine(StartSecondaryFireCoroutine());
            return true;
        }
        return false;
    }

    public bool EndSecondaryFire() {
        if (SecondaryInputStatus != InputStatus.Released && SecondaryInputStatus != InputStatus.NoInput) {
            StartCoroutine(EndSecondaryFireCoroutine());
            return true;
        }
        return false;
    }

    public void InterrruptFire() {
        PrimaryInputStatus = InputStatus.NoInput;
        SecondaryInputStatus = InputStatus.NoInput;
    }

    IEnumerator StartPrimaryFireCoroutine() {
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Pressed;
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Held;
    }

    IEnumerator EndPrimaryFireCoroutine() {
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Released;
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.NoInput;
    }

    IEnumerator StartSecondaryFireCoroutine() {
        yield return new WaitForFixedUpdate();
        SecondaryInputStatus = InputStatus.Pressed;
        yield return new WaitForFixedUpdate();
        SecondaryInputStatus = InputStatus.Held;
    }

    IEnumerator EndSecondaryFireCoroutine() {
        yield return new WaitForFixedUpdate();
        SecondaryInputStatus = InputStatus.Released;
        yield return new WaitForFixedUpdate();
        SecondaryInputStatus = InputStatus.NoInput;
    }

    IEnumerator TimedPrimaryFireCoroutine(float time) {
        // Potential timing issues if I invoke the start/stop coroutines.
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Pressed;
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Held;
        yield return new WaitForSeconds(time);

        // Check that firing wasn't ended early somehow.
        if (PrimaryInputStatus != InputStatus.Released && PrimaryInputStatus != InputStatus.NoInput) {
            yield return new WaitForFixedUpdate();
            PrimaryInputStatus = InputStatus.Released;
            yield return new WaitForFixedUpdate();
            PrimaryInputStatus = InputStatus.NoInput;
        }
    }

    IEnumerator TimedSecondaryFireCoroutine(float time) {
        // Potential timing issues if I invoke the start/stop coroutines.
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Pressed;
        yield return new WaitForFixedUpdate();
        PrimaryInputStatus = InputStatus.Held;
        yield return new WaitForSeconds(time);

        // Check that firing wasn't ended early somehow.
        if (SecondaryInputStatus != InputStatus.Released && SecondaryInputStatus != InputStatus.NoInput) {
            yield return new WaitForFixedUpdate();
            PrimaryInputStatus = InputStatus.Released;
            yield return new WaitForFixedUpdate();
            PrimaryInputStatus = InputStatus.NoInput;
        }

    }

}