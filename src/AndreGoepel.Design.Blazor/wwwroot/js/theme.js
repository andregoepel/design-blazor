/*
 * Design-system theme switcher.
 *
 * Resolves a user preference of "auto" | "light" | "dark" into a concrete
 * data-theme ("light" | "dark") on <html>, persists the preference in
 * localStorage, and tracks the OS colour scheme while "auto" is selected.
 *
 * Exposed on window.agTheme so Blazor JS interop and the inline
 * no-flash bootstrap in App.razor can both use it.
 */
(function () {
  const KEY = "ag-theme";
  const media = window.matchMedia("(prefers-color-scheme: light)");

  function preference() {
    const stored = localStorage.getItem(KEY);
    return stored === "light" || stored === "dark" || stored === "auto"
      ? stored
      : "auto";
  }

  function resolve(pref) {
    if (pref === "auto") {
      return media.matches ? "light" : "dark";
    }
    return pref;
  }

  function apply(pref) {
    document.documentElement.setAttribute("data-theme", resolve(pref));
  }

  // Re-resolve when the OS theme changes, but only while "auto" is active.
  media.addEventListener("change", function () {
    if (preference() === "auto") {
      apply("auto");
    }
  });

  const api = {
    /** Current stored preference: "auto" | "light" | "dark". */
    get: preference,
    /** Concrete theme currently applied: "light" | "dark". */
    resolved: function () {
      return resolve(preference());
    },
    /** Persist and apply a new preference. */
    set: function (pref) {
      const value =
        pref === "light" || pref === "dark" || pref === "auto" ? pref : "auto";
      localStorage.setItem(KEY, value);
      apply(value);
      return value;
    },
    /** Apply the stored preference (no persistence change). */
    init: function () {
      apply(preference());
      return preference();
    },
  };

  window.agTheme = api;
  // Apply immediately so the first paint already matches the preference.
  apply(preference());

  // Blazor's enhanced navigation morphs <html> to the server response, which has
  // no data-theme — wiping the attribute on every page change and reverting the
  // shell to the default theme. Re-apply whenever data-theme diverges from the
  // resolved preference. Runs before paint (microtask), so there's no flash, and
  // it self-terminates because re-setting to the desired value is a no-op.
  new MutationObserver(function () {
    var desired = resolve(preference());
    if (document.documentElement.getAttribute("data-theme") !== desired) {
      document.documentElement.setAttribute("data-theme", desired);
    }
  }).observe(document.documentElement, {
    attributes: true,
    attributeFilter: ["data-theme"],
  });
})();
