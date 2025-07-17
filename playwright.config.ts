import { defineConfig, devices } from '@playwright/test';

/**
 * See https://playwright.dev/docs/test-configuration.
 */
export default defineConfig({
  testDir: './tests/e2e',
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: process.env.CI ? 2 : 0,
  /* Opt out of parallel tests on CI. */
  workers: process.env.CI ? 1 : undefined,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [
    ['html', { open: 'never' }],
    ['json', { outputFile: 'test-results/results.json' }],
    ['junit', { outputFile: 'test-results/results.xml' }],
    ['github'],
    ['dot'],
  ],
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* Base URL to use in actions like `await page.goto('/')`. */
    baseURL: process.env.BASE_URL || 'http://localhost:5000',

    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: 'on-first-retry',
    
    /* Take screenshot on failure */
    screenshot: 'only-on-failure',
    
    /* Record video on failure */
    video: 'retain-on-failure',
    
    /* Timeout for each action */
    actionTimeout: 30000,
    
    /* Timeout for navigation */
    navigationTimeout: 30000,
    
    /* Global timeout for each test */
    timeout: 60000,
    
    /* Locale */
    locale: 'en-US',
    
    /* Timezone */
    timezoneId: 'UTC',
    
    /* Ignore HTTPS errors */
    ignoreHTTPSErrors: true,
    
    /* Accept downloads */
    acceptDownloads: true,
    
    /* Bypass CSP */
    bypassCSP: true,
    
    /* Color scheme */
    colorScheme: 'light',
    
    /* Viewport */
    viewport: { width: 1280, height: 720 },
    
    /* User agent */
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36',
    
    /* Extra HTTP headers */
    extraHTTPHeaders: {
      'Accept-Language': 'en-US,en;q=0.9',
    },
    
    /* Permissions */
    permissions: [],
    
    /* Geolocation */
    geolocation: undefined,
    
    /* Device scale factor */
    deviceScaleFactor: 1,
    
    /* Has touch */
    hasTouch: false,
    
    /* Is mobile */
    isMobile: false,
    
    /* Java script enabled */
    javaScriptEnabled: true,
    
    /* Offline */
    offline: false,
    
    /* Reduced motion */
    reducedMotion: 'no-preference',
    
    /* Force prefers-color-scheme */
    forcedColors: 'none',
    
    /* Storage state */
    storageState: undefined,
    
    /* Service workers */
    serviceWorkers: 'allow',
    
    /* Proxy */
    proxy: undefined,
    
    /* HTTP credentials */
    httpCredentials: undefined,
    
    /* Client certificates */
    clientCertificates: undefined,
    
    /* Strict selectors */
    strictSelectors: false,
    
    /* Test ID attribute */
    testIdAttribute: 'data-testid',
  },

  /* Configure projects for major browsers */
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },

    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },

    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },

    {
      name: 'edge',
      use: { ...devices['Desktop Edge'] },
    },

    /* Test against mobile viewports. */
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
    },
    {
      name: 'Mobile Safari',
      use: { ...devices['iPhone 12'] },
    },

    /* Test against branded browsers. */
    {
      name: 'Microsoft Edge',
      use: { ...devices['Desktop Edge'], channel: 'msedge' },
    },
    {
      name: 'Google Chrome',
      use: { ...devices['Desktop Chrome'], channel: 'chrome' },
    },
  ],

  /* Run your local dev server before starting the tests */
  webServer: [
    {
      command: 'npm run dev',
      url: 'http://localhost:5000',
      reuseExistingServer: !process.env.CI,
      timeout: 120 * 1000,
      stdout: 'ignore',
      stderr: 'pipe',
    },
  ],

  /* Global setup */
  globalSetup: undefined,

  /* Global teardown */
  globalTeardown: undefined,

  /* Expect configuration */
  expect: {
    /* Timeout for expect() calls */
    timeout: 10000,
    
    /* Threshold for screenshot comparison */
    threshold: 0.2,
    
    /* Animation handling */
    toHaveScreenshot: {
      animations: 'disabled',
      caret: 'hide',
    },
    
    /* Page screenshot options */
    toMatchSnapshot: {
      animations: 'disabled',
      caret: 'hide',
    },
  },

  /* Output directory */
  outputDir: 'test-results',

  /* Preserve output directory */
  preserveOutput: 'always',

  /* Update snapshots */
  updateSnapshots: 'missing',

  /* Maximum failures */
  maxFailures: process.env.CI ? 10 : undefined,

  /* Metadata */
  metadata: {
    'test-environment': process.env.NODE_ENV || 'test',
    'base-url': process.env.BASE_URL || 'http://localhost:5000',
    'browser-versions': 'Latest',
    'test-framework': 'Playwright',
    'test-runner': 'Playwright Test Runner',
  },

  /* Quiet mode */
  quiet: false,

  /* Shard */
  shard: undefined,

  /* Grep */
  grep: undefined,

  /* Grep invert */
  grepInvert: undefined,

  /* Update snapshots */
  ignoreSnapshots: false,

  /* Name */
  name: 'MCP Hub E2E Tests',

  /* Project dependencies */
  dependencies: [],

  /* Snapshot path template */
  snapshotPathTemplate: '{testDir}/{testFileDir}/{testFileName}-snapshots/{arg}{ext}',

  /* Test info */
  testInfo: {
    outputPath: 'test-results',
    snapshotSuffix: '',
    project: {
      testDir: './tests/e2e',
      outputDir: 'test-results',
      snapshotDir: './tests/e2e/snapshots',
    },
  },
});