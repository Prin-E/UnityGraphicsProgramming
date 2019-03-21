using UnityEngine;
using System.Collections;

namespace Kodai.Fluid.SPH {

    [RequireComponent(typeof(Fluid2D))]
    public class FluidRenderer : MonoBehaviour {
        public Fluid2D solver;
        public Material RenderParticleMat;
        public Color WaterColor;

        public ComputeBuffer particleBuffer;
        public ComputeShader genParticleShader;

        void Start() {
            if(solver == null)
                solver = GetComponent<Fluid2D>();
            particleBuffer = new ComputeBuffer(solver.NumParticles * 4, 24);
        }

        void LateUpdate() {
            GenerateParticles();
        }

        void OnRenderObject() {
            DrawParticle();
        }

        void OnDestroy() {
            particleBuffer.Release();
        }

        void GenerateParticles() {
            int kernelId = genParticleShader.FindKernel("GenParticleMain");
            genParticleShader.SetBuffer(kernelId, "_ParticlesBufferRead", solver.ParticlesBufferRead);
            genParticleShader.SetBuffer(kernelId, "_ParticlesBufferWrite", particleBuffer);
            genParticleShader.Dispatch(kernelId, solver.NumParticles / 1024, 1, 1);
        }

        void DrawParticle() {
            RenderParticleMat.SetPass(0);
            RenderParticleMat.SetColor("_WaterColor", WaterColor);
            //RenderParticleMat.SetBuffer("_ParticlesBuffer", solver.ParticlesBufferRead);
            RenderParticleMat.SetBuffer("_ParticlesBuffer", particleBuffer);
            Graphics.DrawProcedural(MeshTopology.Quads, solver.NumParticles);
        }
    }
}